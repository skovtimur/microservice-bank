using AccountService.Abstractions.ServiceInterfaces;
using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.DTOs;
using AccountService.Exceptions;
using AutoMapper;
using MediatR;

namespace AccountService.Queries.GetWallet;

public class GetWalletQueryHandler(IMapper mapper)
    : IRequestHandler<GetWalletQuery, WalletDto>
{
    public async Task<WalletDto> Handle(GetWalletQuery request, CancellationToken cancellationToken)
    {
        var foundWallet = WalletsSingleton.Wallets.FirstOrDefault(x => x.Id == request.WalletId);

        if (foundWallet == null)
            throw new NotFoundException(typeof(WalletEntity), request.WalletId);

        if (foundWallet.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException($"The Wallet({request.WalletId}) isn't had by the user({request.OwnerId})");

        var accountId = foundWallet.Id;

        var transactions = TransactionsSingleton.Transactions
            .Where(x => x.AccountId == accountId).ToList();

        var dto = mapper.Map<WalletDto>(foundWallet);
        dto.Transactions = transactions;

        return dto;
    }
}