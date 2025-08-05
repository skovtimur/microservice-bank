using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.Exceptions;
using MediatR;

namespace AccountService.Commands.UpdateWallet;

public class UpdateWalletCommandHandler : IRequestHandler<UpdateWalletCommand>
{
#pragma warning disable // В будущем как добавим бд асинхронность будет уместна
    public async Task Handle(UpdateWalletCommand request, CancellationToken cancellationToken)
#pragma warning restore
    {
        var findIndex = WalletsSingleton.Wallets.FindIndex(x => x.Id == request.Id);

        if (findIndex < 0)
            throw new NotFoundException(typeof(WalletEntity), request.Id);

        var oldWallet = WalletsSingleton.Wallets[findIndex];

        if (oldWallet.IsDeleted)
            throw new BadRequestException("The Account's already deleted");

        if (oldWallet.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException("You're not an owner");

        var updatedWallet = new WalletEntity(oldWallet.Id, oldWallet.CreatedAtUtc, DateTime.UtcNow,
            oldWallet.DeletedAtUtc, oldWallet.IsDeleted, oldWallet.OwnerId, request.NewType, request.NewCurrency,
            oldWallet.OpenedAtUtc, oldWallet.ClosedAtUtc, request.NewInterestRate, oldWallet.Transactions,
            request.NewBalance);
        // TODO 
        // убрать коммент ниже
        // Более геморный способ но за то я сохранил инкапсуляцию 

        WalletsSingleton.Wallets[findIndex] = updatedWallet;
    }
}