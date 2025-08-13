using AccountService.Shared.Exceptions;
using AccountService.Shared.Infrastructure;
using AccountService.Wallets.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Wallets.UpdateWallet;

public class UpdateWalletCommandHandler(IWalletRepository walletRepository)
    : IRequestHandler<UpdateWalletCommand>
{
    public async Task Handle(UpdateWalletCommand request, CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetWithReload(request.Id);

        if (wallet == null)
            throw new NotFoundException(typeof(WalletEntity), request.Id);

        if (wallet.IsDeleted)
            throw new BadRequestException("The Account's already deleted");

        var hasBeenUsed = wallet.Transactions.Count > 0;

        if (hasBeenUsed)
            throw new BadRequestException("You can't update the wallet because it has already been used");

        if (wallet.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException("You're not an owner");

        wallet.CompletelyUpdate(request.NewType, request.NewCurrency, newBalance: request.NewBalance,
            request.ClosedAtUtc,
            newInterestRate: request.NewInterestRate);
        
        await walletRepository.Update(wallet);
    }
}