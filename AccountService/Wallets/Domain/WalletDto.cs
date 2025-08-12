using AccountService.Shared.Domain;
using AccountService.Transactions.Domain;

namespace AccountService.Wallets.Domain;

public class WalletDto
{
#pragma warning disable CS8618, CS9264
    public WalletDto()
#pragma warning restore CS8618, CS9264
    {
        // For Automapper
    }

    public WalletDto(
        Guid id,
        DateTime createdAtUtc,
        DateTime? updatedAtUtc,
        DateTime? deletedAtUtc,
        bool isDeleted,
        Guid ownerId,
        WalletType type,
        CurrencyValueObject currency,
        DateTime openedAtUtc,
        DateTime? closedAtUtc,
        decimal? interestRate,
        List<TransactionDto> transactions,
        decimal balance)
    {
        // ReSharper disable SpecifyACultureInStringConversionExplicitly
        Id = id;
        CreatedAtUtc = createdAtUtc.ToString();
        UpdatedAtUtc = updatedAtUtc.ToString();
        DeletedAtUtc = deletedAtUtc?.ToString();
        IsDeleted = isDeleted;

        OwnerId = ownerId;
        Type = type.ToString();
        Currency = currency.Currency;
        OpenedAtUtc = openedAtUtc.ToString();
        Transactions = transactions;
        Balance = balance;
        InterestRate = interestRate;
        ClosedAtUtc = closedAtUtc?.ToString();
    }
    public Guid Id { get; init; }
    public Guid OwnerId { get; init; }
    public string Type { get; init; }
    public string Currency { get; init; }
    public decimal? InterestRate { get; init; }

    public string OpenedAtUtc { get; init; }
    public string? ClosedAtUtc { get; init; }
    public decimal Balance { get; init; }
    public string CreatedAtUtc { get; init; }
    public string? UpdatedAtUtc { get; init; }
    public string? DeletedAtUtc { get; init; }
    public bool IsDeleted { get; init; }
    public List<TransactionDto> Transactions { get; init; }
}