using AccountService.Features.Transactions.Domain;
using AccountService.Shared.Domain;

namespace AccountService.Features.Wallets.Domain;

public class WalletEntity : BaseEntity
{
#pragma warning disable CS8618, CS9264
    public WalletEntity()
#pragma warning restore CS8618, CS9264
    {
        CreatedAtUtc = DateTime.UtcNow;
        OpenedAtUtc = DateTime.UtcNow;
        Status = AccountStatus.Normal;
    }

    public WalletEntity(Guid id, DateTime createdAtUtc, DateTime? updatedAtUtc, DateTime? deletedAtUtc, bool isDeleted,
        Guid ownerId, WalletType type, CurrencyValueObject currency, DateTime openedAtUtc, DateTime? closedAtUtc,
        decimal? interestRate, List<TransactionEntity> transactions, decimal balance, Guid entityVersion,
        AccountStatus accountStatus = AccountStatus.Normal)
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
        Status = accountStatus;
    }

    public Guid OwnerId { get; init; }
    public WalletType Type { get; private set; }
    public CurrencyValueObject Currency { get; private set; }
    public decimal? InterestRate { get; private set; }

    public DateTime OpenedAtUtc { get; init; }
    public DateTime? ClosedAtUtc { get; private set; }

    public List<TransactionEntity> Transactions { get; init; } = [];
    public decimal Balance { get; private set; }
    public AccountStatus Status { get; private set; }

    public bool IsOwner(Guid userId)
    {
        return OwnerId == userId;
    }

    public bool IsFrozen() => Status == AccountStatus.Frozen;

    public void Freeze()
    {
        Status = AccountStatus.Frozen;
    }

    public void Unfreeze()
    {
        Status = AccountStatus.Normal;
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

    public void UpdateInterestRate(decimal newInterestRate, DateTime newClosedAtUtc)
    {
        InterestRate = newInterestRate;
        ClosedAtUtc = newClosedAtUtc;
    }

    public void CompletelyUpdate(WalletType newType, CurrencyValueObject newCurrency, decimal newBalance,
        DateTime? newClosedAtUtc, decimal? newInterestRate)
    {
        Type = newType;
        ClosedAtUtc = newClosedAtUtc;
        InterestRate = newInterestRate;
        Balance = newBalance;
        Currency = newCurrency;
    }
}