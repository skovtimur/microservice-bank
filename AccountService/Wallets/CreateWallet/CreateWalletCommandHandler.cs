using AccountService.Wallets.Domain;
using AutoMapper;
using MediatR;

namespace AccountService.Wallets.CreateWallet;

public class CreateWalletCommandHandler(IMapper mapper, IWalletRepository walletRepository)
    : IRequestHandler<CreateWalletCommand, Guid>
{
    public async Task<Guid> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        var newWallet = mapper.Map<WalletEntity>(request);
        await walletRepository.Create(newWallet);

        return newWallet.Id;
    }
}