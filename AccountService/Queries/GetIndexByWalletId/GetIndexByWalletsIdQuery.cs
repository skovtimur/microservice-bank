using MediatR;

namespace AccountService.Queries.GetIndexByWalletId;

public class GetIndexByWalletsIdQuery(Guid id) : IRequest<int>
{
    public Guid AccountId { get; set; } = id;
}