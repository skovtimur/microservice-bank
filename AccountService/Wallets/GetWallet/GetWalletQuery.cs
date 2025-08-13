using AccountService.Wallets.Domain;
using MediatR;

namespace AccountService.Wallets.GetWallet;

public class GetWalletQuery(Guid id, Guid ownerId) : IRequest<WalletDto>
{
    public Guid WalletId { get; } = id;
    public Guid OwnerId { get; } = ownerId;
}