using System.ComponentModel.DataAnnotations;
using AccountService.Domain;
using AccountService.Filters;

namespace AccountService.Requests;

[InterestRateRequiredCheckingTypeFilter]
public class WalletUpdateRequest
{
    /// <summary>
    /// Unique identifier of the Account that will be updated
    /// </summary>
    [Required]
    public Guid Id { get; set; }

    /// <summary>
    /// New Type of Wallet
    /// </summary>
    [Required]
    public WalletType NewType { get; set; }

    /// <summary>
    /// New Currency Code in ISO 4217 format. For example: RUB or USD
    /// </summary>
    [Required, StringLength(maximumLength: 3, MinimumLength = 2)]
    public string NewIsoCurrencyCode { get; set; }

    /// <summary>
    /// New Interest Rate (only for Debit and Credit wallets)
    /// </summary>
    [Range(0, 100)]
    public decimal? NewInterestRate { get; set; }

    /// <summary>
    /// New Balance in the updated wallet
    /// </summary>
    [Required, Range(0, int.MaxValue)]
    public decimal NewBalance { get; set; }

    /// <summary>
    /// Wallet will be closed at this new DateTime(UTC)
    /// </summary>
    public DateTime? ClosedAtUtc { get; set; }
}