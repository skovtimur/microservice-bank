using AccountService.Domain.Entities;
using MediatR;

namespace AccountService.Queries.GetTransaction;

public class GetTransactionQuery(Guid id, Guid ownerId) : IRequest<TransactionEntity?>
{
    public Guid Id { get; } = id;
    public Guid OwnerId { get; } = ownerId;
}