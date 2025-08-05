using AccountService.Domain.Entities;
using AccountService.DTOs;
using MediatR;

namespace AccountService.Queries.GetWallet;

public class GetWalletQuery(Guid id, Guid ownerId) : IRequest<WalletDto>
{
    public Guid WalletId { get; set; } = id;
    public Guid OwnerId { get; set; } = ownerId;
}