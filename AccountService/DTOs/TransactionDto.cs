namespace AccountService.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    
    public Guid AccountId { get; set; }
    public Guid? CounterpartyAccountId { get; set; }
    
    public decimal Sum { get; set; }
    public string Currency { get; set; }
    public string TransactionType { get; set; }
    public string Description { get; set; }
    
    public string CreatedAtUtc { get; set; }
    public string? DeletedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}