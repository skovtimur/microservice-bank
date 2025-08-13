using AccountService.Shared.Exceptions;
using AccountService.Wallets.Domain;
using MediatR;

namespace AccountService.Wallets.PartiallyUpdateWallet;

public class PartiallyUpdateWalletCommandHandler(IWalletRepository walletRepository)
    : IRequestHandler<PartiallyUpdateWalletCommand>
{
    public async Task Handle(PartiallyUpdateWalletCommand request, CancellationToken cancellationToken)
    {
        var wallet = await walletRepository.GetWithReload(request.Id);

        if (wallet == null)
            throw new NotFoundException(typeof(WalletEntity), request.Id);

        if (wallet.IsDeleted)
            throw new BadRequestException("The Wallet's deleted");

        if (wallet.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException("You're not an owner");

        if (wallet.Type == WalletType.Checking)
            throw new BadRequestException(
                $"Only Wallet with {WalletType.Deposit} or {WalletType.Credit} type can have an {nameof(WalletEntity.InterestRate)}");

        wallet.UpdateInterestRate(request.NewInterestRate, request.ClosedAtUtc);
        await walletRepository.Update(wallet);
    }
}