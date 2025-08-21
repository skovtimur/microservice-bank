using AccountService.Features.Wallets.Domain;
using AccountService.Shared.RabbitMq.RabbitMqEvents;
using MassTransit;

namespace AccountService.Shared.RabbitMq.Consumers;

// ReSharper disable once ClassNeverInstantiated.Global
// Resharper предлагает прекратить класс в abstract, в этом нет необходимости
public class ClientUnblockedConsumer(
    IWalletRepository walletRepository,
    ILogger<ClientUnblockedConsumer> logger)
    : IConsumer<ClientUnblockedEventModel>
{
    public async Task Consume(ConsumeContext<ClientUnblockedEventModel> context)
    {
        var value = context.Message;
        logger.LogInformation("ClientUnblockedConsumer is consuming. Client Id: {id}", value.ClientId);

        await walletRepository.UnFreezeByOwnerId(value.ClientId);

        logger.LogInformation("ClientUnblockedConsumer consumed. Client Id: {id}", value.ClientId);
    }
}