using AccountService.Transactions.Domain;
using MediatR;

namespace AccountService.Transactions.GetAllTransactions;

public class GetAllTransactionsQuery(Guid accountId, Guid ownerId, DateTime fromAtUtc) : IRequest<List<TransactionDto>>
{
    public Guid AccountId { get; } = accountId;
    public Guid OwnerId { get; } = ownerId;
    public DateTime FromAtUtc { get; } = fromAtUtc;
}