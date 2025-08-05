using System.ComponentModel.DataAnnotations;
using AccountService.Domain;
using AccountService.Filters;

namespace AccountService.Requests;

[InterestRateRequiredCheckingTypeFilter]
public class WalletCreateRequest
{
    /// <summary>
    /// Type of Wallet
    /// </summary>
    [Required]
    public WalletType Type { get; set; }

    /// <summary>
    /// Currency Code in ISO 4217 format. For example: RUB or USD
    /// </summary>
    [Required, StringLength(maximumLength: 3, MinimumLength = 2)]
    public string IsoCurrency { get; set; }

    /// <summary>
    /// New Interest Rate (only for Debit and Credit wallets)
    /// </summary>
    [Range(0, 100)]
    public decimal? InterestRate { get; set; }

    /// <summary>
    /// Balance in the new wallet
    /// </summary>
    [Required, Range(0, int.MaxValue)]
    public decimal Balance { get; set; }

    /// <summary>
    /// Wallet will be closed at this DateTime(UTC)
    /// </summary>
    public DateTime? ClosedAtUtc { get; set; }
}