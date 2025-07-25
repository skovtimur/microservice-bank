using AccountService.Data;
using AccountService.Domain.Entities;
using MediatR;

namespace AccountService.Queries.GetTransaction;

public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionEntity?>
{
    public async Task<TransactionEntity?> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        var index = TransactionsSingleton.Transactions.FindIndex(x => x.Id == request.Id);
        return index >= 0 ? TransactionsSingleton.Transactions[index] : null;
    }
}