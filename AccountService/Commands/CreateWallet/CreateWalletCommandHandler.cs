using AccountService.Abstractions.ServiceInterfaces;
using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.Exceptions;
using AutoMapper;
using MediatR;

namespace AccountService.Commands.CreateWallet;

public class CreateWalletCommandHandler(IMapper mapper)
    : IRequestHandler<CreateWalletCommand, Guid>
{
    public async Task<Guid> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        var newWallet = mapper.Map<WalletEntity>(request);
        WalletsSingleton.Wallets.Add(newWallet);

        return newWallet.Id;
    }
}