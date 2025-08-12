using AccountService.Shared.Exceptions;
using AccountService.Shared.Infrastructure;
using AccountService.Wallets.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Wallets.UpdateWallet;

public class UpdateWalletCommandHandler(IWalletRepository walletRepository, MainDbContext dbContext)
    : IRequestHandler<UpdateWalletCommand>
{
    public async Task Handle(UpdateWalletCommand request, CancellationToken cancellationToken)
    {
        var oldWallet = await walletRepository.Get(request.Id);

        if (oldWallet == null)
            throw new NotFoundException(typeof(WalletEntity), request.Id);

        if (oldWallet.IsDeleted)
            throw new BadRequestException("The Account's already deleted");

        var hasBeenUsed = oldWallet.Transactions.Count > 0;

        if (hasBeenUsed)
            throw new BadRequestException("You can't update the wallet because it has already been used");

        if (oldWallet.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException("You're not an owner");

        dbContext.Entry(oldWallet).State =
            EntityState.Detached; // для того чтобы ef core не орал на то что я пытаюсь поменять на другой обьект уже существующий

        var updatedWallet = new WalletEntity(id: oldWallet.Id, oldWallet.CreatedAtUtc, DateTime.UtcNow,
            oldWallet.DeletedAtUtc, oldWallet.IsDeleted, ownerId: oldWallet.OwnerId, request.NewType,
            request.NewCurrency,
            oldWallet.OpenedAtUtc, oldWallet.ClosedAtUtc, request.NewInterestRate, oldWallet.Transactions,
            request.NewBalance, entityVersion: oldWallet.EntityVersion);

        await walletRepository.Update(updatedWallet);
    }
}