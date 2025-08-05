using System.ComponentModel.DataAnnotations;
using AccountService.Validators;

namespace AccountService.Requests;

public class TransferMoneyRequest
{
    /// <summary>
    /// Unique identifier of the Account
    /// </summary>
    [Required]
    public Guid AccountId { get; set; }

    /// <summary>
    /// Unique identifier of the Counterparty Account
    /// </summary>
    [Required]
    public Guid TransferToCounterpartyAccountId { get; set; }

    /// <summary>
    /// The amount that will be used in the transaction
    /// </summary>
    [Required, Range(0, int.MaxValue)]
    public decimal Sum { get; set; }

    /// <summary>
    /// Currency Code in ISO 4217 format. For example: RUB or USD
    /// </summary>
    [StringLength(maximumLength: 3, MinimumLength = 2)]
    public string IsoCurrencyCode { get; set; }

    /// <summary>
    /// The Description of the new transaction
    /// </summary>
    [Required, StringLength(maximumLength: TransactionCreateCommandValidator.MaxDescriptionLength, MinimumLength = 3)]
    public string Description { get; set; }
}