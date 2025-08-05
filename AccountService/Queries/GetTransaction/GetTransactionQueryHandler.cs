using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.Exceptions;
using MediatR;

namespace AccountService.Queries.GetTransaction;

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionEntity?>
{
    public async Task<TransactionEntity?> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        var index = TransactionsSingleton.Transactions.FindIndex(x => x.Id == request.Id);

        var transaction = index >= 0 ? TransactionsSingleton.Transactions[index] : null;

        if (transaction != null && transaction.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException("You can't see these transactions because you aren't owner");

        return transaction;
    }
}