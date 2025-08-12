using System.Data;
using AccountService.Shared.Exceptions;
using AccountService.Transactions.Domain;
using AccountService.Wallets.Domain;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AccountService.Shared.Infrastructure.Repositories;

public class TransactionRepository(
    MainDbContext dbContext,
    IMapper mapper,
    ILogger<TransactionRepository> logger) : ITransactionRepository
{
    public async Task<TransactionEntity?> Get(Guid id)
    {
        var transaction = await dbContext.Transactions
            .FirstOrDefaultAsync(x => x.Id == id);

        return transaction;
    }

    public async Task<List<TransactionDto>> GetAllByAccountId(Guid accountId, Guid ownerId, DateTime fromAtUtc)
    {
        var transactionEntities = await dbContext.Transactions
            .Where(x => x.IsDeleted == false)
            .Where(x => x.OwnerId == ownerId)
            .Where(x => x.AccountId == accountId)
            .Where(x => x.CreatedAtUtc >= fromAtUtc)
            .Select(x => mapper.Map<TransactionDto>(x))
            .ToListAsync();

        return transactionEntities;
    }

    public async Task SaveNewTransaction(TransactionEntity transaction, WalletEntity account)
    {
        var oldBalance = account.Balance;
        try
        {
            await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            await dbContext.Transactions.AddAsync(transaction);
            ChangeBalanceAndSaveInWallet(transaction, account);

            await dbContext.SaveChangesAsync();

            // ReSharper disable once InvertIf
            var accountFromDb = await dbContext.Wallets.SingleAsync(x => x.Id == account.Id);

            if (accountFromDb.IsBalanceChangeValid(oldBalance, transaction.Sum, transaction.TransactionType) == false)
                throw new DbUpdateException("The Balance wasn't updated");

            await dbContext.Database.CommitTransactionAsync();
        }
        catch (InvalidOperationException e)
        {
            if (e.InnerException is DbUpdateException
                {
                    InnerException: PostgresException { SqlState: "40001" } postgresException
                })
            {
                throw new DbUpdateConcurrencyException(postgresException.Message, postgresException);
            }

            throw;
        }
        catch (Exception e)
        {
            await dbContext.Database.RollbackTransactionAsync();
            logger.LogError("{message}", e.Message);
            throw;
        }
    }

    public async Task SaveNewTransaction(TransactionEntity transaction, WalletEntity account,
        WalletEntity counterpartyAccount)
    {
        var oldBalance = account.Balance;
        var oldCounterpartyBalance = counterpartyAccount.Balance;

        if (transaction.TransactionType == TransactionType.Credit)
        {
            if (counterpartyAccount.Balance < transaction.Sum)
                throw new PaymentRequiredException($"Counterparty Account's Balance is less than {transaction.Sum}");

            if (transaction.TransactionType == TransactionType.Credit &&
                counterpartyAccount.IsOwner(transaction.OwnerId) == false)
            {
                throw new ForbiddenException(
                    $"You can't take someone else's money. Counterparty Account ({transaction.CounterpartyAccountId}) isn't your ");
            }
        }

        try
        {
            await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            var secondTransaction = new TransactionEntity(
                accountId: counterpartyAccount.Id,
                ownerId: counterpartyAccount.OwnerId,
                counterpartyAccountId: transaction.AccountId,
                sum: transaction.Sum,
                currency: transaction.Currency,
                description: transaction.Description,
                transactionType: transaction.TransactionType == TransactionType.Credit
                    ? TransactionType.Debit
                    : TransactionType.Credit
            );
            await dbContext.Transactions.AddAsync(transaction);
            ChangeBalanceAndSaveInWallet(transaction, account);

            await dbContext.Transactions.AddAsync(secondTransaction);
            ChangeBalanceAndSaveInWallet(secondTransaction, counterpartyAccount);

            await dbContext.SaveChangesAsync();

            // ReSharper disable once InvertIf
            var accountFromDb = await dbContext.Wallets.SingleAsync(x => x.Id == account.Id);
            var counterpartyAccountFromDb = await dbContext.Wallets.SingleAsync(x => x.Id == counterpartyAccount.Id);

            // ReSharper disable once InvertIf
            if (accountFromDb.IsBalanceChangeValid(oldBalance, transaction.Sum, transaction.TransactionType) &&
                counterpartyAccountFromDb.IsBalanceChangeValid(oldCounterpartyBalance, secondTransaction.Sum,
                    secondTransaction.TransactionType))
            {
                await dbContext.Database.CommitTransactionAsync();
                return;
            }

            throw new DbUpdateException("The Balance wasn't updated");
        }
        catch (InvalidOperationException e)
        {
            if (e.InnerException is DbUpdateException
                {
                    InnerException: PostgresException { SqlState: "40001" } postgresException
                })
            {
                throw new DbUpdateConcurrencyException(postgresException.Message, postgresException);
            }

            throw;
        }
        catch (Exception e)
        {
            await dbContext.Database.RollbackTransactionAsync();
            logger.LogError("{message}", e.Message);

            throw;
        }
    }

    private void ChangeBalanceAndSaveInWallet(TransactionEntity transaction,
        WalletEntity wallet)
    {
        // ReSharper disable once ConvertIfStatementToSwitchStatement
        // Не вижу смысл переводить в switch если конструкция if выгледит лучше

        if (transaction.TransactionType == TransactionType.Credit)
        {
            wallet.AddMoney(transaction.Sum);
        }
        else if (transaction.TransactionType == TransactionType.Debit)
        {
            wallet.TakeMoney(transaction.Sum);
        }

        // Save in a wallet
        wallet.AddTransaction(transaction);
        wallet.UpdateEntity();

        dbContext.Wallets.Update(wallet);
    }
}