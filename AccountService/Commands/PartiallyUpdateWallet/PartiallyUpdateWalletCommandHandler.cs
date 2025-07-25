using AccountService.Data;
using AccountService.Domain;
using AccountService.Domain.Entities;
using AccountService.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace AccountService.Commands.PartiallyUpdateWallet;

public class PartiallyUpdateWalletCommandHandler : IRequestHandler<PartiallyUpdateWalletCommand>
{
    public async Task Handle(PartiallyUpdateWalletCommand request, CancellationToken cancellationToken)
    {
        var index = WalletsSingleton.Wallets.FindIndex(w => w.Id == request.Id);

        if (index < 0)
            throw new NotFoundException(typeof(WalletEntity), request.Id);

        var wallet = WalletsSingleton.Wallets[index];

        if (wallet.IsDeleted)
            throw new BadRequestExсeption("The Wallet's deleted");

        if ((wallet.Type == WalletType.Checking && request.NewInterestRate == null)
            || (wallet.Type != WalletType.Checking && request.NewInterestRate != null))
        {
            wallet.InterestRate = request.NewInterestRate;
            wallet.ClosedAtUtc = request.ClosedAtUtc;
            wallet.UpdatedAtUtc = DateTime.UtcNow;
            WalletsSingleton.Wallets[index] = wallet;

            return;
        }

        throw new BadRequestExсeption(
            $"Only Wallet with {WalletType.Deposit} or {WalletType.Credit} type can have an {nameof(WalletEntity.InterestRate)}");
    }
}