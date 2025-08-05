using AccountService.Domain;
using AccountService.Domain.ValueObjects;
using AccountService.DTOs;
using AccountService.Validators;
using MediatR;

namespace AccountService.Commands.CreateTransaction;

public class TransactionCreateCommand : IRequest<Guid>
{
    public required Guid OwnerId { get; init; }
    public required Guid AccountId { get; init; }
    // ReSharper disable once MemberCanBePrivate.Global
    public Guid? CounterpartyAccountId { get; init; }
    public required decimal Sum { get; init; }
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    //Обе подсказки на TransactionType и CounterpartyAccountId думаю лишние 
    public required TransactionType TransactionType { get; set; }

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

        return result.IsSuccess
            ? MbResult<TransactionCreateCommand>.Ok(newTransaction)
            : MbResult<TransactionCreateCommand>.Fail(result.ErrorMessage);
    }
}