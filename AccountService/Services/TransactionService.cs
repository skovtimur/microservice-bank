using AccountService.Abstractions.ServiceInterfaces;
using AccountService.Data;
using AccountService.Domain;
using AccountService.Domain.Entities;
using AccountService.Exceptions;
using AccountService.Queries.GetIndexByWalletId;
using MediatR;

namespace AccountService.Services;

public class TransactionService(IMediator mediator) : ITransactionService
{
    public async Task SaveNewTransaction(TransactionEntity transaction, Guid ownerId)
    {
        // 1. Save only one
        if (transaction.CounterpartyAccountId == null)
        {
            TransactionsSingleton.Transactions.Add(transaction);
            await ChangeBalanceAndSaveInWallet(transaction);

            return;
        }

        // 2.0 If CounterpartyAccountId isn't null:
        // 2.1 Checking  
        var counterpartyAccountIndex =
            await mediator.Send(new GetIndexByWalletsIdQuery((Guid)transaction.CounterpartyAccountId));

        if (counterpartyAccountIndex < 0)
            throw new NotFoundException(
                $"The Counterparty Account ({transaction.CounterpartyAccountId}) wasn't found");

        var counterpartyAccount = WalletsSingleton.Wallets[counterpartyAccountIndex];

        if (counterpartyAccount.IsDeleted)
            throw new BadRequestExсeption("The Counterparty Account's already deleted");

        // 2.2 Create a second transaction too:
        var secondTransaction = new TransactionEntity((Guid)transaction.CounterpartyAccountId,
            transaction.Sum, transaction.Currency, transaction.Description,
            transaction.TransactionType == TransactionType.Credit
                ? TransactionType.Debit
                : TransactionType.Credit,
            transaction.AccountId, counterpartyAccount.OwnerId);

        // 2.3 User can't take someone else's money
        if (secondTransaction.TransactionType == TransactionType.Debit)
        {
            if (counterpartyAccount.Balance < transaction.Sum)
                throw new PaymentRequiredException($"Counterparty Account's Balance is less than {transaction.Sum}");

            if (secondTransaction.TransactionType == TransactionType.Debit &&
                counterpartyAccount.IsOwner(ownerId) == false)
                throw new ForbiddenException(
                    $"You can't take someone else's money. Counterparty Account ({transaction.CounterpartyAccountId}) isn't your ");
        }

        // 2.4 Save both ones
        TransactionsSingleton.Transactions.AddRange(transaction, secondTransaction);
        await ChangeBalanceAndSaveInWallet(transaction);
        await ChangeBalanceAndSaveInWallet(secondTransaction);

        // TODO
        // удалить комент ниже
        //Тяжело назвать это полноценной атомарной операцией, но после добавление реальной бд это будет полноценной транзакцией в базу 
    }

    private async Task ChangeBalanceAndSaveInWallet(TransactionEntity transaction)
    {
        var walletIndex = await mediator.Send(new GetIndexByWalletsIdQuery(transaction.AccountId));

        if (walletIndex < 0)
            throw new NotFoundException(typeof(WalletEntity), transaction.AccountId);

        var wallet = WalletsSingleton.Wallets[walletIndex];

        // Add or take money
        if (transaction.TransactionType == TransactionType.Credit)
        {
            wallet.AddMoney(transaction.Sum);
        }
        else if (transaction.TransactionType == TransactionType.Debit)
        {
            wallet.TakeMoney(transaction.Sum);
        }

        // Save in a wallet
        wallet.AddTransaction(transaction);
        WalletsSingleton.Wallets[walletIndex] = wallet;
    }
}