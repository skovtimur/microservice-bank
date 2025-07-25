using AccountService.Domain.Entities;
using AccountService.DTOs;
using MediatR;

namespace AccountService.Queries.GetAllWallets;

public class GetAllWallestQuery(Guid ownerId) : IRequest<List<WalletDto>>
{
    public Guid OwnerId { get; set; } = ownerId;
}