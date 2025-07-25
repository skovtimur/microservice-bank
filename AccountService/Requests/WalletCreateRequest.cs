using System.ComponentModel.DataAnnotations;
using AccountService.Domain;
using AccountService.Filters;

namespace AccountService.Requests;

[InterestRateRequiredCheckingTypeFilter]
public class WalletCreateRequest
{
    [Required] public Guid OwnerId { get; set; }
    [Required]  public WalletType Type { get; set; }


    [Required, StringLength(maximumLength: 3, MinimumLength = 2)]
    public string IsoCurrency { get; set; }

    [Range(0, 100)] public decimal? InterestRate { get; set; }
    [Required, Range(0, int.MaxValue)] public decimal Balance { get; set; }
    
    public DateTime? ClosedAtUtc { get; set; }
}