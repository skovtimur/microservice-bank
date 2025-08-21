using AccountService.Features.Wallets.Domain;
using AccountService.Shared.Abstractions;
using AccountService.Shared.Domain;

namespace AccountService.Shared.RabbitMq.RabbitMqEvents;

// ReSharper disable UnusedMember.Global
//AccountOpened { eventId, occurredAt, accountId, ownerId, currency, type }
public class AccountOpenedEventModel(
    Guid eventId,
    Guid accountId,
    Guid ownerId,
    WalletType type,
    CurrencyValueObject currency,
    DateTime openedAt) : BaseRabbitEvent(eventId, openedAt)
{
    public Guid AccountId { get; init; } = accountId;
    public Guid OwnerId { get; init; } = ownerId;

    public string Type { get; init; } = type.ToString();
    public string Currency { get; init; } = currency.Currency;
}