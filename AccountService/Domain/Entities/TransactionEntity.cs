using AccountService.Domain.ValueObjects;

namespace AccountService.Domain.Entities;

public class TransactionEntity : BaseEntity
{
    public Guid AccountId { get; set; }
    public Guid? CounterpartyAccountId { get; set; } //Участник сделки, например кому переводишь
    public decimal Sum { get; set; }
    public CurrencyValueObject Currency { get; set; }
    public TransactionType TransactionType { get; set; }
    public DescriptionValueObject Description { get; set; }
}