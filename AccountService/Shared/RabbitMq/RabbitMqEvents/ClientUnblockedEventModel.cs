using AccountService.Shared.Abstractions;

namespace AccountService.Shared.RabbitMq.RabbitMqEvents;

// ReSharper disable once ClassNeverInstantiated.Global
// Resharper предлагает сделать класс abstract, это не нужно
public class ClientUnblockedEventModel(Guid eventId, DateTime occurredAt, Guid clientId) : BaseRabbitEvent(eventId, occurredAt)
{
    public Guid ClientId { get; } = clientId;
}