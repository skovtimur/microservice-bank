using AccountService.Domain.Entities;
using MediatR;

namespace AccountService.Queries.GetWallet;

public class GetWalletQuery(Guid id, Guid ownerId) : IRequest<WalletEntity>
{
    public Guid WalletId { get; set; } = id;
    public Guid OwnerId { get; set; } = ownerId;
}