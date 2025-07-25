using System.ComponentModel.DataAnnotations;
using AccountService.Domain;
using AccountService.Filters;

namespace AccountService.Requests;

[InterestRateRequiredCheckingTypeFilter]
public class WalletUpdateRequest
{
    [Required] public Guid Id { get; set; }
    [Required] public WalletType NewType { get; set; }

    [Required, StringLength(maximumLength: 3, MinimumLength = 2)]
    public string NewIsoCurrencyCode { get; set; }

    [Range(0, 100)] public decimal? NewInterestRate { get; set; }
    [Required, Range(0, int.MaxValue)] public decimal NewBalance { get; set; }
    public DateTime? ClosedAtUtc { get; set; }
}