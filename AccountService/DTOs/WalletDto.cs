using AccountService.Domain.Entities;

namespace AccountService.DTOs;

public class WalletDto(
    Guid id,
    Guid ownerId,
    string type,
    string currency,
    decimal? interestRate,
    string openedAtUtc,
    string? closedAtUtc,
    List<TransactionEntity> transactions,
    decimal balance,
    string createdAtUtc,
    string? updatedAtUtc,
    string? deletedAtUtc,
    bool isDeleted)
{
    public Guid Id { get; init; } = id;

    public Guid OwnerId { get; init; } = ownerId;
    public string Type { get; init; } = type;
    public string Currency { get; init; } = currency;
    public decimal? InterestRate { get; init; } = interestRate;

    public string OpenedAtUtc { get; init; } = openedAtUtc;
    public string? ClosedAtUtc { get; init; } = closedAtUtc;
    public decimal Balance { get; init; } = balance;
    public string CreatedAtUtc { get; init; } = createdAtUtc;
    public string? UpdatedAtUtc { get; init; } = updatedAtUtc;
    public string? DeletedAtUtc { get; init; } = deletedAtUtc;
    public bool IsDeleted { get; init; } = isDeleted;


    private List<TransactionEntity> _transactions = transactions;
    public List<TransactionEntity> Transactions
    {
        // ReSharper disable once UnusedMember.Global
        // Getter скорее всего пригодится в будущем
        get => _transactions;
        set => _transactions = value ?? throw new ArgumentNullException(nameof(value));
    }
}