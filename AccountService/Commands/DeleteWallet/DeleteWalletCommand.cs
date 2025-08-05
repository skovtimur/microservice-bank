using MediatR;

namespace AccountService.Commands.DeleteWallet;

public class DeleteWalletCommand(Guid walletId, Guid ownerId) : IRequest
{
    public Guid WalletId { get; } = walletId;
    public Guid OwnerId { get; } = ownerId;
}