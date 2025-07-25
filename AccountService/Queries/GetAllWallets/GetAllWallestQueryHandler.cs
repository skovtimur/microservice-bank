using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.DTOs;
using AutoMapper;
using MediatR;

namespace AccountService.Queries.GetAllWallets;

public class GetAllWallestQueryHandler(IMapper mapper) : IRequestHandler<GetAllWallestQuery, List<WalletDto>>
{
    public async Task<List<WalletDto>> Handle(GetAllWallestQuery request, CancellationToken cancellationToken)
    {
        var dtos = new List<WalletDto>();
        
        var wallets = WalletsSingleton.Wallets
            .Where(x => x.OwnerId == request.OwnerId)
            .Where(x => x.IsDeleted == false)
            .ToList();

        // Тут так же в будущем заменю на Include если будем использовать EF Core
        foreach (var wallet in wallets)
        {
            var accountId = wallet.Id;

            var transactions = TransactionsSingleton.Transactions
                .Where(x => x.AccountId == accountId).ToList();

            wallet.Transactions = transactions;

            var dto = mapper.Map<WalletDto>(wallet);
            dtos.Add(dto);
        }

        return dtos;
    }
}