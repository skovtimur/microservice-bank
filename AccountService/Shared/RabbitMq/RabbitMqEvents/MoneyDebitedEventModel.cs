using AccountService.Shared.Abstractions;
using AccountService.Shared.Domain;

namespace AccountService.Shared.RabbitMq.RabbitMqEvents;

// ReSharper disable UnusedMember.Global
//MoneyDebited { eventId, occurredAt, accountId, amount, currency, operationId, reason }
public class MoneyDebitedEventModel(
    Guid eventId,
    Guid accountId,
    Guid transactionId,
    decimal amount,
    CurrencyValueObject currency,
    DateTime debitedAt,
    string reason) : BaseRabbitEvent(eventId, debitedAt)
{
    public Guid AccountId { get; init; } = accountId;
    public Guid OperationId { get; init; } = transactionId;

    public decimal Amount { get; init; } = amount;
    public string Currency { get; init; } = currency.Currency;
    public string Reason { get; init; } = reason;
}