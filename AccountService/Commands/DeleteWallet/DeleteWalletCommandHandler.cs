using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.Exceptions;
using MediatR;

namespace AccountService.Commands.DeleteWallet;

public class DeleteWalletCommandHandler : IRequestHandler<DeleteWalletCommand>
{
    public async Task Handle(DeleteWalletCommand request, CancellationToken cancellationToken)
    {
        var findIndex = WalletsSingleton.Wallets.FindIndex(x => x.Id == request.WalletId);

        if (findIndex < 0)
            throw new NotFoundException(typeof(WalletEntity), request.WalletId);

        var wallet = WalletsSingleton.Wallets[findIndex];
        
        if (wallet.IsDeleted)
            throw new BadRequestExсeption("The Wallet's already deleted");
        
        if (wallet.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException("You're not an owner");
        
        wallet.IsDeleted = true;
        wallet.DeletedAtUtc = DateTime.UtcNow;

        WalletsSingleton.Wallets[findIndex] = wallet;
    }
}