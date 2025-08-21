using AccountService.Shared.Abstractions;
using AccountService.Shared.Domain;

namespace AccountService.Shared.RabbitMq.RabbitMqEvents;

// ReSharper disable UnusedMember.Global
//MoneyCredited { eventId, occurredAt, accountId, amount, currency, operationId }
public class MoneyCreditedEventModel(
    Guid eventId,
    Guid accountId,
    Guid transactionId,
    decimal amount,
    CurrencyValueObject currency,
    DateTime creditedAt) : BaseRabbitEvent(eventId, creditedAt)
{
    public Guid AccountId { get; init; } = accountId;
    public Guid OperationId { get; init; } = transactionId;

    public decimal Amount { get; init; } = amount;
    public string Currency { get; init; } = currency.Currency;
}