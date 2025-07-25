using AccountService.Data;
using MediatR;

namespace AccountService.Queries.GetIndexByWalletId;

public class GetIndexByWalletsIdQueryHandler : IRequestHandler<GetIndexByWalletsIdQuery, int>
{
    public async Task<int> Handle(GetIndexByWalletsIdQuery request, CancellationToken cancellationToken) =>
        WalletsSingleton.Wallets.FindIndex(x => x.Id == request.AccountId);
}