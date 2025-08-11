using AccountService.Abstractions.ServiceInterfaces;
using AccountService.Data;
using AccountService.Domain;
using AccountService.Domain.Entities;
using AccountService.Exceptions;
using AccountService.Queries.GetIndexByWalletId;
using AutoMapper;
using MediatR;

namespace AccountService.Commands.CreateTransaction;

public class TransactionCreateCommandHandler(
    IMapper mapper,
    IMediator mediator,
    ITransactionService transactionService)
    : IRequestHandler<TransactionCreateCommand, Guid>
{
    public async Task<Guid> Handle(TransactionCreateCommand request, CancellationToken cancellationToken)
    {
        // 1) Mapping to create an Entity
        var transaction = mapper.Map<TransactionEntity>(request);

        // 2) Get index of the Account
        var accountIndex =
            await mediator.Send(new GetIndexByWalletsIdQuery(request.AccountId), cancellationToken);

        if (accountIndex < 0)
            throw new NotFoundException($"The Account ({request.AccountId}) wasn't found");

        // 3) Checking if the user is owner
        var account = WalletsSingleton.Wallets[accountIndex];

        if (account.IsDeleted)
            throw new BadRequestException("The Account's already deleted");

        if (account.IsOwner(request.OwnerId) == false)
            throw new ForbiddenException($"You aren't an owner of this account ({request.AccountId})");

        // 4) Balance Checking
        if (transaction.TransactionType == TransactionType.Debit && account.Balance < request.Sum)
            throw new PaymentRequiredException($"Account's Balance is less than {request.Sum}");

        // 5) Save a transaction
        await transactionService.SaveNewTransaction(transaction, request.OwnerId);
        return transaction.Id;
    }
}