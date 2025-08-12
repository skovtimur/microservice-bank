namespace AccountService.Shared.Domain;

public abstract class BaseEntity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();

    public DateTime CreatedAtUtc { get; protected init; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; protected set; }
    public DateTime? DeletedAtUtc { get; protected set; }
    public bool IsDeleted { get; protected set; }
    public Guid EntityVersion { get; protected set; } = Guid.NewGuid();

    public void UpdateEntity()
    {
        UpdatedAtUtc = DateTime.UtcNow;
        EntityVersion = Guid.NewGuid();
    }

    public void DeleteEntitySoftly()
    {
        DeletedAtUtc = DateTime.UtcNow;
        IsDeleted = true;
    }
}