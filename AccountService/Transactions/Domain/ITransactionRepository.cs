using AccountService.Wallets.Domain;

namespace AccountService.Transactions.Domain;

public interface ITransactionRepository
{
    public Task<TransactionEntity?> Get(Guid id);
    public Task<List<TransactionDto>> GetAllByAccountId(Guid accountId, Guid ownerId, DateTime fromAtUtc);

    public Task SaveNewTransaction(TransactionEntity transaction, WalletEntity account);

    public Task SaveNewTransaction(TransactionEntity transaction, WalletEntity account,
        WalletEntity counterpartyAccount);
}