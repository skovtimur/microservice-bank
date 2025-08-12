using AccountService.Shared.Domain;
using AccountService.Transactions.Domain;

namespace AccountService.Wallets.Domain;

public class WalletEntity : BaseEntity
{
#pragma warning disable CS8618, CS9264
    public WalletEntity()
#pragma warning restore CS8618, CS9264
    {
        CreatedAtUtc = DateTime.UtcNow;
        OpenedAtUtc = DateTime.UtcNow;
    }

    public WalletEntity(Guid id, DateTime createdAtUtc, DateTime? updatedAtUtc, DateTime? deletedAtUtc, bool isDeleted,
        Guid ownerId, WalletType type, CurrencyValueObject currency, DateTime openedAtUtc, DateTime? closedAtUtc,
        decimal? interestRate, List<TransactionEntity> transactions, decimal balance, Guid entityVersion)
    {
        Id = id;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = updatedAtUtc;
        DeletedAtUtc = deletedAtUtc;
        IsDeleted = isDeleted;
        EntityVersion = entityVersion;

        OwnerId = ownerId;
        Type = type;
        Currency = currency;
        OpenedAtUtc = openedAtUtc;
        Transactions = transactions;
        Balance = balance;
        InterestRate = interestRate;
        ClosedAtUtc = closedAtUtc;
    }

    public Guid OwnerId { get; init; }
    public WalletType Type { get; init; }
    public CurrencyValueObject Currency { get; init; }
    public decimal? InterestRate { get; set; }

    public DateTime OpenedAtUtc { get; init; }
    public DateTime? ClosedAtUtc { get; set; }

    public List<TransactionEntity> Transactions { get; init; } = [];
    public decimal Balance { get; private set; }

    public bool IsOwner(Guid userId)
    {
        return OwnerId == userId;
    }

    public void AddTransaction(TransactionEntity transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);
        Transactions.Add(transaction);
    }

    public void AddMoney(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        Balance += amount;
    }

    public void TakeMoney(decimal amount)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(amount);
        Balance -= amount;
    }

    public bool IsBalanceChangeValid(decimal oldBalance, decimal sum, TransactionType sumType)
    {
        if (sumType == TransactionType.Debit)
            sum *= -1;

        return Balance - oldBalance == sum;
    }

    public void AccrualInterest()
    {
        if (InterestRate == null)
            throw new InvalidOperationException("Interest rate cannot be null");

        var valueOfOnePercent = Balance / 100;
        var sum = valueOfOnePercent * (decimal)InterestRate;

        Balance += sum;
    }

    public void UpdateInterestRate(decimal newInterestRate, DateTime newClosedAtUtc)
    {
        InterestRate = newInterestRate;
        ClosedAtUtc = newClosedAtUtc;
    }
}