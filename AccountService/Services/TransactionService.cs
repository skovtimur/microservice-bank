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
    public async Task CreateNewTransaction(TransactionEntity transaction)
    {
        var walletIndex = await mediator.Send(new GetIndexByWalletsIdQuery(transaction.AccountId));

        if (walletIndex < 0)
            throw new NotFoundException(typeof(WalletEntity), transaction.AccountId);

        var wallet = WalletsSingleton.Wallets[walletIndex];

        if (transaction.TransactionType == TransactionType.Credit)
        {
            wallet.Balance += transaction.Sum;
        }
        else if (transaction.TransactionType == TransactionType.Debit)
        {
            wallet.Balance -= transaction.Sum;
        }
        
        wallet.Transactions.Add(transaction);
        WalletsSingleton.Wallets[walletIndex] = wallet;
        TransactionsSingleton.Transactions.Add(transaction);
    }
}