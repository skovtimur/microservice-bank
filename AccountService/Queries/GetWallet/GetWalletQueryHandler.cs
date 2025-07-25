using AccountService.Abstractions.ServiceInterfaces;
using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.Exceptions;
using MediatR;

namespace AccountService.Queries.GetWallet;

public class GetWalletQueryHandler(IUserService userService) : IRequestHandler<GetWalletQuery, WalletEntity>
{
    public async Task<WalletEntity> Handle(GetWalletQuery request, CancellationToken cancellationToken)
    {
        var isUserExist = await userService.IsUserExistAlwaysReturnTrue(request.OwnerId);

        if (isUserExist == false)
            throw new NotFoundException($"The user with Id={request.OwnerId} doesn't exist");

        var foundWallet = WalletsSingleton.Wallets.FirstOrDefault(x => x.Id == request.WalletId);

        if (foundWallet == null)
            throw new NotFoundException(typeof(WalletEntity), request.WalletId);

        if (foundWallet.OwnerId != request.OwnerId)
            throw new ForbiddenException($"The Wallet({request.WalletId}) isn't had by the user({request.OwnerId})");


        var accountId = foundWallet.Id;

        var transactions = TransactionsSingleton.Transactions
            .Where(x => x.AccountId == accountId).ToList();

        foundWallet.Transactions = transactions; //Это я заменю на Include (если далее будем работать с EF Core-ом)

        return foundWallet;
    }
}