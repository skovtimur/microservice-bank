using AccountService.Features.Wallets.Domain;
using AccountService.Shared.Exceptions;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Wallets.GetWallet;

public class GetWalletQueryHandler(IMapper mapper, IWalletRepository walletRepository)
    : IRequestHandler<GetWalletQuery, WalletDto>
{
    public async Task<WalletDto> Handle(GetWalletQuery request, CancellationToken cancellationToken)
    {
        var foundWallet = await walletRepository.Get(request.WalletId);

        if (foundWallet == null)
            throw new NotFoundException(typeof(WalletEntity), request.WalletId);

        if (foundWallet.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException($"The Wallet({request.WalletId}) isn't had by the user({request.OwnerId})");

        var dto = mapper.Map<WalletDto>(foundWallet);
        return dto;
    }
}