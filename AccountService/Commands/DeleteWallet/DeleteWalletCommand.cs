using MediatR;

namespace AccountService.Commands.DeleteWallet;

public class DeleteWalletCommand(Guid walletId) : IRequest
{
    public Guid WalletId { get; set; } = walletId;
}