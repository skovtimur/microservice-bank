using AccountService.Features.Wallets.Domain;
using MediatR;

namespace AccountService.Features.Wallets.GetWallet;

public class GetWalletQuery(Guid id, Guid ownerId) : IRequest<WalletDto>
{
    public Guid WalletId { get; } = id;
    public Guid OwnerId { get; } = ownerId;
}