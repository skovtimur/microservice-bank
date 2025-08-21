using AccountService.Features.Wallets.Domain;
using AccountService.Shared.RabbitMq.RabbitMqEvents;
using AutoMapper;
using MassTransit;
using MediatR;

namespace AccountService.Features.Wallets.CreateWallet;

public class CreateWalletCommandHandler(
    IMapper mapper,
    IWalletRepository walletRepository,
    IPublishEndpoint publishEndpoint)
    : IRequestHandler<CreateWalletCommand, Guid>
{
    public async Task<Guid> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        var newWallet = mapper.Map<WalletEntity>(request);
        
        await publishEndpoint.Publish(new AccountOpenedEventModel(Guid.NewGuid(), newWallet.Id, newWallet.OwnerId,
            newWallet.Type, newWallet.Currency, newWallet.OpenedAtUtc), cancellationToken);

        await walletRepository.Create(newWallet);
        return newWallet.Id;
    }
}