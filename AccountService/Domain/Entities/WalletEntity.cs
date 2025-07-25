using AccountService.Domain.ValueObjects;

namespace AccountService.Domain.Entities;

public class WalletEntity : BaseEntity
{
    public Guid OwnerId { get; set; }
    public WalletType Type { get; set; }
    public CurrencyValueObject Currency { get; set; }
    public decimal? InterestRate { get; set; }

    public DateTime OpenedAtUtc { get; set; }
    public DateTime? ClosedAtUtc { get; set; }

    public List<TransactionEntity> Transactions { get; set; } = [];
    public decimal Balance { get; set; }
}