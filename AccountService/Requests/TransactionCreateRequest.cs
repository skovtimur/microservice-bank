using System.ComponentModel.DataAnnotations;
using AccountService.Domain;
using AccountService.Domain.ValueObjects;

namespace AccountService.Requests;

public class TransactionCreateRequest
{
    [Required] public Guid OwnerId { get; set; }
    [Required] public Guid AccountId { get; set; }
    public Guid? CounterpartyAccountId { get; set; }
    [Required, Range(0, int.MaxValue)] public decimal Sum { get; set; }
    [Required] public TransactionType TransactionType { get; set; }

    [StringLength(maximumLength: 3, MinimumLength = 2)]
    public string IsoCurrencyCode { get; set; }

    [Required, StringLength(maximumLength: MaxDescriptionLength, MinimumLength = 3)]
    public string Description { get; set; }

    public const int MaxDescriptionLength = 5000;
}