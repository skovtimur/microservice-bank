using System.ComponentModel.DataAnnotations;
using AccountService.Domain;
using AccountService.Validators;

namespace AccountService.Requests;

public class TransactionCreateRequest
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    
    /// <summary>
    /// Unique identifier of the Account. The main member in the transaction
    /// </summary>
    [Required]
    public required Guid AccountId { get; set; }

    /// <summary>
    /// Unique identifier of the Counterparty Account. Other member in the transaction
    /// </summary>
    public Guid? CounterpartyAccountId { get; set; }

    /// <summary>
    /// The amount that will be used in the transaction
    /// </summary>
    [Required, Range(0, int.MaxValue)]
    public required  decimal Sum { get; set; }

    /// <summary>
    /// Transaction Type
    /// </summary>
    [Required]
    public required  TransactionType TransactionType { get; set; }

    /// <summary>
    /// Currency Code in ISO 4217 format. For example: RUB or USD
    /// </summary>
    [StringLength(maximumLength: 3, MinimumLength = 2)]
    public required string IsoCurrencyCode { get; set; }

    /// <summary>
    /// The Description of the new transaction
    /// </summary>
    [Required, StringLength(maximumLength: TransactionCreateCommandValidator.MaxDescriptionLength, MinimumLength = 3)]
    public required string Description { get; set; }
}