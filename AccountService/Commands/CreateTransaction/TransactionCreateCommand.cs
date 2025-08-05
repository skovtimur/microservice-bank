using AccountService.Commands.CreateWallet;
using AccountService.Domain;
using AccountService.Domain.ValueObjects;
using AccountService.DTOs;
using AccountService.Validators;
using MediatR;

namespace AccountService.Commands.CreateTransaction;

public class TransactionCreateCommand : IRequest<Guid>
{
    public Guid OwnerId { get; set; }
    public Guid AccountId { get; set; }
    public Guid? CounterpartyAccountId { get; set; }
    public decimal Sum { get; set; }
    public TransactionType TransactionType { get; set; }

    public CurrencyValueObject Currency { get; set; }

    public DescriptionValueObject Description { get; set; }

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
                : counterpartyAccountId,
        };
        var result = TransactionCreateCommandValidator.IsValid(newTransaction);

        return result.IsSuccess
            ? MbResult<TransactionCreateCommand>.Ok(newTransaction)
            : MbResult<TransactionCreateCommand>.Fail(result.ErrorMessage);
    }
}