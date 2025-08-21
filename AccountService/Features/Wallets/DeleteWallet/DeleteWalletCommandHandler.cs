using AccountService.Features.Wallets.Domain;
using AccountService.Shared.Exceptions;
using MediatR;

namespace AccountService.Features.Wallets.DeleteWallet;

public class DeleteWalletCommandHandler(IWalletRepository walletRepository) : IRequestHandler<DeleteWalletCommand>
{
    public async Task Handle(DeleteWalletCommand request, CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.Get(request.WalletId);

        if (wallet == null)
            throw new NotFoundException(typeof(WalletEntity), request.WalletId);

        if (wallet.IsDeleted)
            throw new BadRequestException("The Wallet's already deleted");

        if (wallet.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException("You're not an owner");
        
        await walletRepository.Delete(wallet);
    }
}