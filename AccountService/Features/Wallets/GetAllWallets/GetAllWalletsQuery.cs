using AccountService.Features.Wallets.Domain;
using MediatR;

namespace AccountService.Features.Wallets.GetAllWallets;

public class GetAllWalletsQuery(Guid ownerId) : IRequest<List<WalletDto>>
{
    public Guid OwnerId { get; } = ownerId;
}