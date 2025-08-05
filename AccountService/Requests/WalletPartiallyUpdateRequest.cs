using System.ComponentModel.DataAnnotations;

namespace AccountService.Requests;

public class WalletPartiallyUpdateRequest
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global

    /// <summary>
    /// Unique identifier of the Account that will be updated
    /// </summary>
    [Required]
    public required Guid Id { get; set; }

    /// <summary>
    /// New Currency Code in ISO 4217 format. For example: RUB or USD
    /// </summary>
    [Required, Range(0, 100)]
    public required decimal NewInterestRate { get; set; }

    /// <summary>
    /// New Closed DateTime(UTC)
    /// </summary>
    [Required]
    public required DateTime ClosedAtUtc { get; set; }
}