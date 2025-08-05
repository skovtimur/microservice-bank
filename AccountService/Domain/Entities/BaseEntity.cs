namespace AccountService.Domain.Entities;

public class BaseEntity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; protected init; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
}