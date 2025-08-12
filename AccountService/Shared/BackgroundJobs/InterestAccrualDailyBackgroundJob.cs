using AccountService.Shared.Abstractions.BackgroundJobInterfaces;
using AccountService.Shared.Infrastructure;
using AccountService.Wallets.Domain;
using Hangfire;

namespace AccountService.Shared.BackgroundJobs;

public class InterestAccrualDailyBackgroundJob(MainDbContext dbContext, IRecurringJobManager manager)
    : IInterestAccrualDailyBackgroundJob
{
    public void Run()
    {
        manager.AddOrUpdate(
            "accrual_interest_to_deposit_wallets",
            () => AccrualAllDepositWallets(),
            Cron.Daily
        );
    }

    // ReSharper disable once MemberCanBePrivate.Global
    // public метод нужен для коректной работы Hangfire
    public void AccrualAllDepositWallets()
    {
        var wallets = dbContext.Wallets
            .Where(x => x.IsDeleted == false)
            .Where(x => x.Type == WalletType.Deposit && x.InterestRate != null)
            .ToList();

        foreach (var wallet in wallets)
            wallet.AccrualInterest();

        dbContext.SaveChanges();
    }
}