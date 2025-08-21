using System.ComponentModel.DataAnnotations;

namespace AccountService.Shared.Options;

// ReSharper disable PropertyCanBeMadeInitOnly.Global
public class RabbitMqOptions
{
    [Required] public required string Host { get; set; }
    [Required] public required string User { get; set; }
    [Required] public required string Password { get; set; }
    [Required] public required string ExchangeName { get; set; }
    [Required] public required int Port { get; set; }

    // Queues:
    [Required] public required string CrmQueue { get; set; }
    [Required] public required string NotificationsQueue { get; set; }
    [Required] public required string AntifraudQueue { get; set; }

    //Routing key patters:
    [Required] public required string AccountKeyPattern { get; set; }
    [Required] public required string MoneyKeyPattern { get; set; }
    [Required] public required string ClientKeyPattern { get; set; }


    //Routing Keys:
    [Required] public required string AccountOpenedRoutingKey { get; set; }
    [Required] public required string MoneyCreditedRoutingKey { get; set; }
    [Required] public required string MoneyDebitedRoutingKey { get; set; }
    [Required] public required string TransferCompletedRoutingKey { get; set; }
    [Required] public required string InterestAccruedRoutingKey { get; set; }
    [Required] public required string ClientBlockedRoutingKey { get; set; }
    [Required] public required string ClientUnblockedRoutingKey { get; set; }
}