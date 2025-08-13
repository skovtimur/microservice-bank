using AccountService.Wallets.Domain;
using MediatR;

namespace AccountService.Wallets.GetAllWallets;

public class GetAllWalletsQuery(Guid ownerId) : IRequest<List<WalletDto>>
{
    public Guid OwnerId { get; } = ownerId;
}