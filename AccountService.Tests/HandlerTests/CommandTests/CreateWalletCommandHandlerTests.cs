using AccountService.Shared.Domain;
using AccountService.Wallets.CreateWallet;
using AccountService.Wallets.Domain;
using AutoMapper;
using Moq;

namespace AccountService.Tests.HandlerTests.CommandTests;

public class CreateWalletCommandHandlerTests
{
    [Theory]
    [InlineData(WalletType.Checking, "RUB", 9999, null)]
    [InlineData(WalletType.Credit, "PHP", 1000, 3)]
    [InlineData(WalletType.Deposit, "KZT", 4, 6)]
    public async Task Handle_Should_Map_Request_And_Creates_Wallet(WalletType type, string currencyValue,
        int balance, int? interestRate)
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var currencyValueObject = new CurrencyValueObject { Currency = currencyValue };

        var request = new CreateWalletCommand
        {
            OwnerId = ownerId,
            Type = type,
            Currency = currencyValueObject,
            InterestRate = interestRate,
            Balance = balance,
            ClosedAtUtc = null
        };

        var responseWalletEntity = new WalletEntity(walletId, DateTime.UtcNow, null, null, false,
            ownerId, type, currencyValueObject, DateTime.UtcNow, null,
            interestRate, [], balance, entityVersion: Guid.NewGuid());

        var mapperMock = new Mock<IMapper>();

        mapperMock
            .Setup(m => m.Map<WalletEntity>(request))
            .Returns(responseWalletEntity);

        var walletRepositoryMock = new Mock<IWalletRepository>();

        walletRepositoryMock
            .Setup(r => r.Create(responseWalletEntity))
            .Returns(Task.CompletedTask);

        var handler = new CreateWalletCommandHandler(mapperMock.Object, walletRepositoryMock.Object);

        // Act
        var result = await handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.Equal(walletId, result);
    }
}