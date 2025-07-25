using System.ComponentModel.DataAnnotations;

namespace AccountService.Requests;

public class WalletPartiallyUpdateRequest
{
    [Required] public Guid Id { get; set; }
    [Required, Range(0, 100)] public decimal NewInterestRate { get; set; }
    [Required] public DateTime ClosedAtUtc { get; set; }
}