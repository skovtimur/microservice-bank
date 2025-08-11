using AccountService.Domain.Entities;

namespace AccountService.Abstractions.ServiceInterfaces;

public interface ITransactionService
{
    public Task SaveNewTransaction(TransactionEntity transaction, Guid ownerId);
}