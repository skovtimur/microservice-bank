using AccountService.Features.Transactions.Domain;
using MediatR;

namespace AccountService.Features.Transactions.GetTransaction;

public class GetTransactionQuery(Guid id, Guid ownerId) : IRequest<TransactionDto?>
{
    public Guid Id { get; } = id;
    public Guid OwnerId { get; } = ownerId;
}