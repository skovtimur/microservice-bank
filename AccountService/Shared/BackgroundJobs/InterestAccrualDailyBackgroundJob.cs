using AccountService.Features.Wallets.Domain;
using AccountService.Shared.Abstractions.BackgroundJobInterfaces;
using AccountService.Shared.Infrastructure;
using AccountService.Shared.RabbitMq.RabbitMqEvents;
using Hangfire;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Shared.BackgroundJobs;

public class InterestAccrualDailyBackgroundJob(
    MainDbContext dbContext,
    IRecurringJobManager manager,
    IPublishEndpoint publishEndpoint)
    : IInterestAccrualDailyBackgroundJob
{
    private const string AccrueInterestProcedureName = "accrue_interest";

    // ReSharper disable once UseRawString
    public const string CreateOrReplaceAccrueInterestProcedureCommand = $@"CREATE EXTENSION IF NOT EXISTS pgcrypto;
CREATE OR REPLACE PROCEDURE {AccrueInterestProcedureName}(p_wallet_id UUID)
LANGUAGE plpgsql
AS $$
DECLARE
    v_owner_id       uuid;
    v_balance        numeric;
    v_interest_rate  numeric;
    v_currency       text;
    v_interest       numeric;
BEGIN
    -- 1) Читаем кошелёк
    SELECT w.owner_id, w.balance, w.interest_rate, w.currency
      INTO v_owner_id, v_balance, v_interest_rate, v_currency
      FROM wallets w
     WHERE w.id = p_wallet_id
       AND w.is_deleted = false
       AND w.interest_rate IS NOT NULL
       AND w.type = 1
     FOR UPDATE;

    IF NOT FOUND THEN
        RAISE EXCEPTION 'Wallet % not found or not eligible for interest accrual', p_wallet_id
            USING ERRCODE = 'P0002';
    END IF;

    -- 2) Логика AccrualInterest():
    v_interest := ROUND((v_balance / 100) * v_interest_rate, 2);

    -- Если начисление нулевое – просто выходим (без шума)
    IF v_interest = 0 THEN
        RETURN;
    END IF;

    -- 3) Создаем нужную транзакцию
    INSERT INTO transactions (
        id,
        created_at_utc,
        updated_at_utc,
        deleted_at_utc,
        is_deleted,
        entity_version,
        account_id,
        owner_id,
        counterparty_account_id,
        transaction_type,
        sum,
        currency,
        description
    )
    VALUES (
        gen_random_uuid(),               -- id
        (now() AT TIME ZONE 'UTC'),      -- created_at_utc
        NULL,                            -- updated_at_utc
        NULL,                            -- deleted_at_utc
        false,                           -- is_deleted
        gen_random_uuid(),               -- entity_version
        p_wallet_id,                     -- account_id
        v_owner_id,                      -- owner_id
        NULL,                            -- counterparty_account_id
        0,                               -- transaction_type
        v_interest,                      -- sum
        v_currency,                      -- currency
        'Interest accrual Description'   -- description (можно локализовать)
    );

    -- 4) Обновляем баланс кошелька
    UPDATE wallets
       SET balance        = balance + v_interest,
           updated_at_utc = (now() AT TIME ZONE 'UTC'),
           entity_version = gen_random_uuid()
     WHERE id = p_wallet_id;
END;
$$; ";

    public Task Run()
    {
        manager.AddOrUpdate(
            "accrual_interest_to_deposit_wallets",
            () => AccrualAllDepositWallets(),
            Cron.Daily
        );

        return Task.CompletedTask;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    // public метод нужен для коректной работы Hangfire
    public void AccrualAllDepositWallets()
    {
        using var tx = dbContext.Database.BeginTransaction();
        var wallets = dbContext.Wallets
            .Where(w => w.IsDeleted == false
                        && w.InterestRate != null
                        && w.Type == WalletType.Deposit)
            .ToList();

        foreach (var w in wallets)
        {
            dbContext.Database.ExecuteSqlRaw($"CALL {AccrueInterestProcedureName}({{0}})", w.Id);

            var task = publishEndpoint.Publish(new InterestAccruedEventModel(Guid.NewGuid(), w.Id, w.Balance, DateTime.UtcNow,
                DateTime.UtcNow, DateTime.UtcNow.AddDays(1)));
            
            Task.WaitAll(task);
        }

        tx.Commit();
    }
}