using MediatR;

namespace AccountService.Wallets.DeleteWallet;

public class DeleteWalletCommand(Guid walletId, Guid ownerId) : IRequest
{
    public Guid WalletId { get; } = walletId;
    public Guid OwnerId { get; } = ownerId;
}