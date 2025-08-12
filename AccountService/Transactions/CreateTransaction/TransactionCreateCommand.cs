using AccountService.Shared.Domain;
using AccountService.Transactions.Domain;
using MediatR;

namespace AccountService.Transactions.CreateTransaction;

public class TransactionCreateCommand : IRequest<Guid>
{
    public required Guid OwnerId { get; init; }
    public required Guid AccountId { get; init; }
    public Guid? CounterpartyAccountId { get; init; }
    public required decimal Sum { get; init; }
    public required TransactionType TransactionType { get; init; }
    public required CurrencyValueObject Currency { get; init; }
    public required DescriptionValueObject Description { get; init; }

    public static MbResult<TransactionCreateCommand> Create(Guid ownerId, Guid accountId, decimal sum,
        TransactionType transactionType, CurrencyValueObject currency, DescriptionValueObject description,
        Guid? counterpartyAccountId)
    {
        var newTransaction = new TransactionCreateCommand
        {
            OwnerId = ownerId,
            AccountId = accountId,
            Sum = sum,
            TransactionType = transactionType,
            Currency = currency,
            Description = description,

            CounterpartyAccountId = counterpartyAccountId == null || counterpartyAccountId == Guid.Empty
                ? null
                : counterpartyAccountId
        };
        var result = TransactionCreateCommandValidator.IsValid(newTransaction);
        
        // ReSharper disable once NullableWarningSuppressionIsUsed
        // раз не валидно значит есть причина почему
        
        return result.IsSuccess
            ? MbResult<TransactionCreateCommand>.Ok(newTransaction)
            : MbResult<TransactionCreateCommand>.Fail(result.Error!);
    }
}