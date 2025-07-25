using AccountService.Domain.Entities;

namespace AccountService.DTOs;

public class WalletDto
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }
    public string Type { get; set; }
    public string Currency { get; set; }
    public decimal? InterestRate { get; set; }

    public string OpenedAtUtc { get; set; }
    public string? ClosedAtUtc { get; set; }

    public List<TransactionEntity> Transactions { get; set; }
    public decimal Balance { get; set; }

    public string CreatedAtUtc { get; set; }
    public string? UpdatedAtUtc { get; set; }
    public string? DeletedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}