using AccountService.Domain.Entities;
using MediatR;

namespace AccountService.Queries.GetTransaction;

public class GetTransactionQuery(Guid id) : IRequest<TransactionEntity?>
{
    public Guid Id { get; set; } = id;
}