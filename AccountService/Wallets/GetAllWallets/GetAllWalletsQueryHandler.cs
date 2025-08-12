using AccountService.Wallets.Domain;
using MediatR;

namespace AccountService.Wallets.GetAllWallets;

public class GetAllWalletsQueryHandler(IWalletRepository walletRepository) : IRequestHandler<GetAllWalletsQuery, List<WalletDto>>
{
    public async Task<List<WalletDto>> Handle(GetAllWalletsQuery request, CancellationToken cancellationToken)
    {
        var wallets = await walletRepository.GetAllWalletByUserId(request.OwnerId);
        return wallets;
    }
}