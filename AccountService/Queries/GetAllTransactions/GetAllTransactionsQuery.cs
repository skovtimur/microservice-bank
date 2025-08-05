using AccountService.DTOs;
using MediatR;

namespace AccountService.Queries.GetAllTransactions;

public class GetAllTransactionsQuery(Guid accountId, Guid ownerId, DateTime fromAtUtc) : IRequest<List<TransactionDto>>
{
    public Guid AccountId { get; set; } = accountId;
    public Guid OwnerId { get; set; } = ownerId;
    public DateTime FromAtUtc { get; set; } = fromAtUtc;
}