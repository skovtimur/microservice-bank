using System.ComponentModel.DataAnnotations;

namespace AccountService.Requests;

public class TransferMoneyRequest
{
    [Required] public Guid OwnerId { get; set; }
    [Required] public Guid AccountId { get; set; }
    [Required] public Guid TransferToCounterpartyAccountId { get; set; }
    
    [Required, Range(0, int.MaxValue)] public decimal Sum { get; set; }

    [StringLength(maximumLength: 3, MinimumLength = 2)]
    public string IsoCurrencyCode { get; set; }

    [Required, StringLength(maximumLength: TransactionCreateRequest.MaxDescriptionLength, MinimumLength = 3)]
    public string Description { get; set; }
}