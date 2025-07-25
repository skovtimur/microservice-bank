using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.Exceptions;
using MediatR;

namespace AccountService.Commands.UpdateWallet;

public class UpdateWalletCommandHandler : IRequestHandler<UpdateWalletCommand>
{
    public async Task Handle(UpdateWalletCommand request, CancellationToken cancellationToken)
    {
        var findIndex = WalletsSingleton.Wallets.FindIndex(x => x.Id == request.Id);

        if (findIndex < 0)
            throw new NotFoundException(typeof(WalletEntity), request.Id);

        var updatedWallet = WalletsSingleton.Wallets[findIndex];

        if (updatedWallet.IsDeleted)
            throw new BadRequestExсeption("The Account's already deleted");

        updatedWallet.Type = request.NewType;
        updatedWallet.Currency = request.NewCurrency;
        updatedWallet.Balance = request.NewBalance;
        updatedWallet.InterestRate = request.NewInterestRate;
        updatedWallet.UpdatedAtUtc = DateTime.UtcNow;
        
        WalletsSingleton.Wallets[findIndex] = updatedWallet;
    }
}