using AccountService.DTOs;
using MediatR;

namespace AccountService.Queries.GetAllWallets;

public class GetAllWalletsQuery(Guid ownerId) : IRequest<List<WalletDto>>
{
    public Guid OwnerId { get; } = ownerId;
}