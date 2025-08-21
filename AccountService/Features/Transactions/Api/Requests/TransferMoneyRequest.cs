using System.ComponentModel.DataAnnotations;
using AccountService.Shared.Domain;

namespace AccountService.Features.Transactions.Api.Requests;

public class TransferMoneyRequest
{
    // ReSharper disable UnusedAutoPropertyAccessor.Global
    // ReSharper disable PropertyCanBeMadeInitOnly.Global
    // public set для того чтобы Binding работал

    /// <summary>
    /// Unique identifier of the Account
    /// </summary>
    [Required]
    public required Guid AccountId { get; set; }

    /// <summary>
    /// Unique identifier of the Counterparty Account
    /// </summary>
    [Required]
    public required Guid TransferToCounterpartyAccountId { get; set; }

    /// <summary>
    /// The amount that will be used in the transaction
    /// </summary>
    [Required, Range(0, int.MaxValue)]
    public required decimal Sum { get; set; }

    /// <summary>
    /// Currency Code in ISO 4217 format. For example: RUB or USD
    /// </summary>
    [StringLength(maximumLength: 3, MinimumLength = 2)]
    public required string IsoCurrencyCode { get; set; }

    /// <summary>
    /// The Description of the new transaction
    /// </summary>
    [Required,
     StringLength(maximumLength: DescriptionValidator.MaxDescriptionLength,
         MinimumLength = DescriptionValidator.MinDescriptionLength)]
    public required string Description { get; set; }
}