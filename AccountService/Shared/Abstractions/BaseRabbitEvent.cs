namespace AccountService.Shared.Abstractions;

// ReSharper disable UnusedMember.Global
public abstract class BaseRabbitEvent(Guid eventId, DateTime occurredAt)
{
    public Guid EventId { get; init; } =  eventId;
    public DateTime OccurredAt { get; init; } = occurredAt;
}