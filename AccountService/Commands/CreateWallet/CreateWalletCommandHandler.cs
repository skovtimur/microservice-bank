using AccountService.Abstractions.ServiceInterfaces;
using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.Exceptions;
using AutoMapper;
using MediatR;

namespace AccountService.Commands.CreateWallet;

public class CreateWalletCommandHandler(IMapper mapper, IUserService userService) : IRequestHandler<CreateWalletCommand, Guid>
{
    public async Task<Guid> Handle(CreateWalletCommand request, CancellationToken cancellationToken)
    {
        var isUserExist = await userService.IsUserExistAlwaysReturnTrue(request.OwnerId);

        if (isUserExist == false)
            throw new NotFoundException($"The User with Id: {request.OwnerId} doesn't exist");
        
        var newWallet = mapper.Map<WalletEntity>(request);
        newWallet.CreatedAtUtc = DateTime.UtcNow;
        newWallet.OpenedAtUtc = DateTime.UtcNow;

        WalletsSingleton.Wallets.Add(newWallet);

        return newWallet.Id;
    }
}