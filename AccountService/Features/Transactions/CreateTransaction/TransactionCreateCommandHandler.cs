using AccountService.Features.Transactions.Domain;
using AccountService.Features.Wallets.Domain;
using AccountService.Shared.Exceptions;
using AutoMapper;
using MediatR;

namespace AccountService.Features.Transactions.CreateTransaction;

public class TransactionCreateCommandHandler(
    IMapper mapper,
    IWalletRepository walletRepository,
    ITransactionRepository transactionRepository,
    ILogger<TransactionCreateCommandHandler> logger)
    : IRequestHandler<TransactionCreateCommand, Guid>
{
    public async Task<Guid> Handle(TransactionCreateCommand request, CancellationToken cancellationToken)
    {
        // 1.0 Mapping to create an Entity
        var transaction = mapper.Map<TransactionEntity>(request);

        // 1.1 Get the Account
        var account = await walletRepository.Get(transaction.AccountId);

        if (account == null)
            throw new NotFoundException($"The Account ({request.AccountId}) wasn't found");

        // 1.2 Checking if the user is owner
        if (account.IsDeleted)
            throw new BadRequestException("The Account's already deleted");

        if (account.IsOwner(request.OwnerId) == false)
        {
            throw new ForbiddenException($"You aren't an owner of this account ({request.AccountId})");
        }
        if (account.IsFrozen())
        {
            throw new ForbiddenException($"Your Account({request.AccountId}) have been frozen");
        }

        // 1.3 Balance Checking
        if (transaction.TransactionType == TransactionType.Debit && account.Balance < request.Sum)
            throw new PaymentRequiredException($"Account's Balance is less than {request.Sum}");

        // 1.4 Save a transaction
        if (transaction.CounterpartyAccountId == null
            || transaction.CounterpartyAccountId == Guid.Empty)
        {
            await transactionRepository.SaveNewTransaction(transaction, account);
            return transaction.Id;
        }

        // If the transaction have a counterparty id:
        // 2.0 Get Counterparty Account
        var counterpartyAccount = await walletRepository.Get((Guid)transaction.CounterpartyAccountId);

        if (counterpartyAccount == null)
            throw new NotFoundException(
                $"The Counterparty Account ({transaction.CounterpartyAccountId}) wasn't found");

        if (counterpartyAccount.IsDeleted)
            throw new BadRequestException("The Counterparty Account's already deleted");

        await transactionRepository.SaveNewTransaction(transaction, account: account,
            counterpartyAccount: counterpartyAccount);

        logger.LogTrace("The new {type} Transaction({id}) has created", transaction.TransactionType, transaction.Id);
        return transaction.Id;
    }
}