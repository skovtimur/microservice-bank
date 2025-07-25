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
    ILogger<TransactionCreateCommandHandler> logger,
    IMapper mapper,
    IMediator mediator,
    ITransactionService transactionService)
    : IRequestHandler<TransactionCreateCommand, Guid>
{
    public async Task<Guid> Handle(TransactionCreateCommand request, CancellationToken cancellationToken)
    {
        // 1) Маппим создавая Entity / Mapping to create an Entity
        var firstTransaction = mapper.Map<TransactionEntity>(request);

        // 2) Берем индексы Account / Get index of the Account
        var accountIndex =
            await mediator.Send(new GetIndexByWalletsIdQuery(request.AccountId));

        if (accountIndex < 0)
            throw new NotFoundException($"The Account ({request.AccountId}) wasn't found");

        // 3) Проверяем является ли юзер владельцем / Checking if the user is owner
        var account = WalletsSingleton.Wallets[accountIndex];

        if (account.IsDeleted)
            throw new BadRequestExсeption("The Account's already deleted");

        if (account.OwnerId != request.OwnerId)
            throw new ForbiddenException($"You aren't an owner of this account ({request.AccountId})");

        if (firstTransaction.TransactionType == TransactionType.Debit && account.Balance < request.Sum)
            throw new PaymentRequiredException($"Account's Balance is less than {request.Sum}");

        firstTransaction.CounterpartyAccountId = null;
        // 4) If The Counterparty account isn't null, it creates a new transaction and mirrors the one 
        if (request.CounterpartyAccountId != null && request.CounterpartyAccountId != Guid.Empty)
        {
            // Меняем значения для 2-й, как бы отзеркаливаем
            // Если первая была debit то вторая  должна быть credit для counterparty 1-й транзакции
            var secondTransaction = mapper.Map<TransactionEntity>(request);

            secondTransaction.AccountId = (Guid)request.CounterpartyAccountId;
            secondTransaction.CounterpartyAccountId = request.AccountId;
            secondTransaction.TransactionType =
                request.TransactionType == TransactionType.Credit
                    ? TransactionType.Debit
                    : TransactionType.Credit;
            
            var counterpartyAccountIndex =
                await mediator.Send(new GetIndexByWalletsIdQuery(secondTransaction.AccountId));

            if (counterpartyAccountIndex < 0)
                throw new NotFoundException(
                    $"The Counterparty Account ({secondTransaction.AccountId}) wasn't found");

            var counterpartyAccount = WalletsSingleton.Wallets[counterpartyAccountIndex];

            if (counterpartyAccount.IsDeleted)
                throw new BadRequestExсeption("The Counterparty Account's already deleted");

            if (secondTransaction.TransactionType == TransactionType.Debit && counterpartyAccount.Balance < request.Sum)
                throw new PaymentRequiredException($"Counterparty Account's Balance is less than {request.Sum}");
            
            // You can't take someone else's money
            if (secondTransaction.TransactionType == TransactionType.Debit &&
                counterpartyAccount.OwnerId != request.OwnerId)
                throw new ForbiddenException(
                    $"You can't take someone else's money. Counterparty Account ({request.CounterpartyAccountId}) isn't your ");

            firstTransaction.CounterpartyAccountId = secondTransaction.AccountId;
            await transactionService.CreateNewTransaction(secondTransaction);
        }

        // 5) Add first transaction
        await transactionService.CreateNewTransaction(firstTransaction);
        return firstTransaction.Id;
    }
}