using AccountService.Shared.Domain;

namespace AccountService.Features.Transactions.Domain;

public class TransactionEntity(
    Guid accountId,
    Guid ownerId,
    Guid? counterpartyAccountId,
    decimal sum,
    CurrencyValueObject currency,
    DescriptionValueObject description,
    TransactionType transactionType)
    : BaseEntity
{
    public Guid AccountId { get; init; } = accountId;

    public Guid? CounterpartyAccountId { get; init; } = counterpartyAccountId;

    public decimal Sum { get; init; } = sum;
    public CurrencyValueObject Currency { get; init; } = currency;
    public TransactionType TransactionType { get; init; } = transactionType;
    public DescriptionValueObject Description { get; init; } = description;
    public Guid OwnerId { get; init; } = ownerId;

    public bool IsOwner(Guid userId) => OwnerId == userId;
}