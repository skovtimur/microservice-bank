namespace AccountService.DTOs;


public class TransactionDto(
    Guid id,
    Guid accountId,
    Guid? counterpartyAccountId,
    decimal sum,
    string currency,
    string transactionType,
    string description,
    string createdAtUtc,
    string? deletedAtUtc,
    bool isDeleted)
{
    public Guid Id { get; init; } = id;

    public Guid AccountId { get; init; } = accountId;
    public Guid? CounterpartyAccountId { get; init; } = counterpartyAccountId;

    public decimal Sum { get; init; } = sum;
    public string Currency { get; init; } = currency;
    public string TransactionType { get; init; } = transactionType;
    public string Description { get; init; } = description;

    public string CreatedAtUtc { get; init; } = createdAtUtc;
    public string? DeletedAtUtc { get; init; } = deletedAtUtc;
    public bool IsDeleted { get; init; } = isDeleted;
}