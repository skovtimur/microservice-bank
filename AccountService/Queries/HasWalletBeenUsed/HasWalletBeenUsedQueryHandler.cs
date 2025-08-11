using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.Exceptions;
using MediatR;

namespace AccountService.Queries.HasWalletBeenUsed;

public class HasWalletBeenUsedQueryHandler : IRequestHandler<HasWalletBeenUsedQuery, bool>
{
#pragma warning disable // Асинхронный метод будет работать асинхронно как мы добавим бд
    public async Task<bool> Handle(HasWalletBeenUsedQuery request, CancellationToken cancellationToken)
    {
        var index = WalletsSingleton.Wallets.FindIndex(x => x.Id == request.Id);

        if (index < 0)
            throw new NotFoundException(typeof(WalletEntity), request.Id);

        var wallet = WalletsSingleton.Wallets[index];
        var transactionCount = TransactionsSingleton.Transactions.Count(x => x.AccountId == wallet.Id);

        return transactionCount > 0;
    }
}