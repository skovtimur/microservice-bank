using AccountService.Domain.ValueObjects;

namespace AccountService.Domain.Entities;

public class TransactionEntity(
    Guid accountId,
    decimal sum,
    CurrencyValueObject currency,
    DescriptionValueObject description,
    TransactionType transactionType,
    Guid ownerId,
    Guid? counterpartyAccountId)
    : BaseEntity
{
    public Guid AccountId { get; init; } = accountId;

    public Guid? CounterpartyAccountId { get; init; } =
        counterpartyAccountId;

    public decimal Sum { get; init; } = sum;
    public CurrencyValueObject Currency { get; init; } = currency;
    public TransactionType TransactionType { get; init; } = transactionType;
    public DescriptionValueObject Description { get; init; } = description;
    public Guid OwnerId { get; init; } = ownerId;

    public bool IsOwner(Guid userId) => OwnerId == userId;
}