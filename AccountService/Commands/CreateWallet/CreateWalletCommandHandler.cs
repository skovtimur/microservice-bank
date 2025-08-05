using AccountService.Data;
using AccountService.Domain.Entities;
using AutoMapper;
using MediatR;

namespace AccountService.Commands.CreateWallet;

public class CreateWalletCommandHandler(IMapper mapper)
    : IRequestHandler<CreateWalletCommand, Guid>
{
#pragma warning disable // После добавления бд асинхронность будет уместна
    public async Task<Guid> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        var newWallet = mapper.Map<WalletEntity>(request);
        WalletsSingleton.Wallets.Add(newWallet);

        return newWallet.Id;
    }
}