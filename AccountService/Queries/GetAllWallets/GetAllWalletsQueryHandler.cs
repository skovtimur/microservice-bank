using AccountService.Data;
using AccountService.DTOs;
using AutoMapper;
using MediatR;

namespace AccountService.Queries.GetAllWallets;

public class GetAllWalletsQueryHandler(IMapper mapper) : IRequestHandler<GetAllWalletsQuery, List<WalletDto>>
{
#pragma warning disable // Асинхронный метод будет работать асинхронно как мы добавим бд
    public async Task<List<WalletDto>> Handle(GetAllWalletsQuery request, CancellationToken cancellationToken)
    {
        var dtos = new List<WalletDto>();

        var wallets = WalletsSingleton.Wallets
            .Where(x => x.OwnerId == request.OwnerId)
            .Where(x => x.IsDeleted == false)
            .ToList();

        // TODO
        // Тут так же в будущем заменю на Include если будем использовать EF Core
        foreach (var wallet in wallets)
        {
            var accountId = wallet.Id;

            var transactions = TransactionsSingleton.Transactions
                .Where(x => x.AccountId == accountId).ToList();

            var dto = mapper.Map<WalletDto>(wallet);
            dto.Transactions = transactions;

            dtos.Add(dto);
        }

        return dtos;
    }
}