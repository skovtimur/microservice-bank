using AccountService.Transactions.Domain;
using MediatR;

namespace AccountService.Transactions.GetAllTransactions;

public class GetAllTransactionsQueryHandler(ITransactionRepository transactionRepository)
    : IRequestHandler<GetAllTransactionsQuery, List<TransactionDto>>
{
    public async Task<List<TransactionDto>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await
            transactionRepository.GetAllByAccountId(request.AccountId, request.OwnerId,
                DateTime.SpecifyKind(request.FromAtUtc, DateTimeKind.Utc));

        return transactions;
    }
}