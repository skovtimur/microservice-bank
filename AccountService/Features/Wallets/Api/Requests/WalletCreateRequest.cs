using System.ComponentModel.DataAnnotations;
using AccountService.Features.Wallets.Domain;
using AccountService.Shared.Api.Filters;

namespace AccountService.Features.Wallets.Api.Requests;

[InterestRateRequiredCheckingTypeFilter]
public class WalletCreateRequest
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    // public set для того чтобы Binding работал

    /// <summary>
    /// Type of Wallet
    /// </summary>
    [Required]
    public required WalletType Type { get; set; }

    /// <summary>
    /// Currency Code in ISO 4217 format. For example: RUB or USD
    /// </summary>
    [Required, StringLength(maximumLength: 3, MinimumLength = 2)]
    public required string IsoCurrency { get; set; }

    /// <summary>
    /// New Interest Rate (only for Debit and Credit wallets)
    /// </summary>
    [Range(0, 100)]
    public decimal? InterestRate { get; set; }

    /// <summary>
    /// Balance in the new wallet
    /// </summary>
    [Required, Range(0, int.MaxValue)]
    public required decimal Balance { get; set; }

    /// <summary>
    /// Wallet will be closed at this DateTime(UTC)
    /// </summary>
    public DateTime? ClosedAtUtc { get; set; }
}