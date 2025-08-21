using System.ComponentModel.DataAnnotations;

namespace AccountService.Features.Wallets.Api.Requests;

public class WalletPartiallyUpdateRequest
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    // public set для того чтобы Binding работал

    /// <summary>
    /// Unique identifier of the Account that will be updated
    /// </summary>
    [Required]
    public required Guid Id { get; set; }

    /// <summary>
    /// New Interest Rate
    /// </summary>
    [Required, Range(0, 100)]
    public required decimal NewInterestRate { get; set; }

    /// <summary>
    /// New Closed DateTime(UTC)
    /// </summary>
    [Required]
    public required DateTime ClosedAtUtc { get; set; }
}