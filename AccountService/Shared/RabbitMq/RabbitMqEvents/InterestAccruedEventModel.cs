using AccountService.Shared.Abstractions;

namespace AccountService.Shared.RabbitMq.RabbitMqEvents;

// ReSharper disable UnusedMember.Global
//InterestAccrued { eventId, occurredAt, accountId, periodFrom, periodTo, amount }
public class InterestAccruedEventModel(
    Guid eventId,
    Guid accountId,
    decimal amount,
    DateTime occurredAt,
    DateTime periodFrom,
    DateTime periodTo) : BaseRabbitEvent(eventId, occurredAt)
{
    public Guid AccountId { get; init; } = accountId;
    public decimal Amount { get; init; } = amount;

    public DateTime PeriodFrom { get; init; } = periodFrom;
    public DateTime PeriodTo { get; init; } = periodTo;
}