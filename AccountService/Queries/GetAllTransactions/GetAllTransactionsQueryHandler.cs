using AccountService.Data;
using AccountService.Domain.Entities;
using AccountService.DTOs;
using AccountService.Exceptions;
using AutoMapper;
using MediatR;

namespace AccountService.Queries.GetAllTransactions;

public class GetAllTransactionsQueryHandler(IMapper mapper)
    : IRequestHandler<GetAllTransactionsQuery, List<TransactionDto>>
{
    public async Task<List<TransactionDto>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var indexOfWallet = WalletsSingleton.Wallets.FindIndex(x => x.Id == request.AccountId);

        if (indexOfWallet < 0)
            throw new NotFoundException(typeof(WalletEntity), request.AccountId);

        var wallet = WalletsSingleton.Wallets[indexOfWallet];

        if (wallet.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException("You can't see these transactions because you aren't owner");

        var transactionEntities = TransactionsSingleton.Transactions
            .Where(x => x.IsDeleted == false)
            .Where(x => x.AccountId == request.AccountId)
            .Where(x => x.CreatedAtUtc >= request.FromAtUtc)
            .Select(mapper.Map<TransactionDto>)
            .ToList();

        return transactionEntities;
    }
}