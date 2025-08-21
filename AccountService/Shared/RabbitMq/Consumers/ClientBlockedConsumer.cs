using AccountService.Features.Wallets.Domain;
using AccountService.Shared.RabbitMq.RabbitMqEvents;
using MassTransit;

namespace AccountService.Shared.RabbitMq.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
// Resharper предлагает прекратить класс в abstract, в этом нет необходимости
public class ClientBlockedConsumer(
    IWalletRepository walletRepository,
    ILogger<ClientUnblockedConsumer> logger)
    : IConsumer<ClientBlockedEventModel>
{
    public async Task Consume(ConsumeContext<ClientBlockedEventModel> context)
    {
        var value = context.Message;
        logger.LogInformation("ClientBlockedConsumer is consuming. Client Id: {id}", value.ClientId);

        await walletRepository.FreezeByOwnerId(value.ClientId);

        logger.LogInformation("ClientBlockedConsumer consumed. Client Id: {id}", value.ClientId);
    }
}