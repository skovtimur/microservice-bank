using AccountService.Features.Wallets.Domain;
using AccountService.Features.Wallets.GetWallet;
using AccountService.Shared.Domain;
using AccountService.Shared.Exceptions;
using AutoMapper;
using Moq;

namespace AccountService.Tests.HandlerTests.QueryTests;

public class GetWalletQueryTests
{
    [Theory]
    [InlineData(WalletType.Checking, "RUB", 9999, null)]
    [InlineData(WalletType.Credit, "PHP", 1000, 3)]
    [InlineData(WalletType.Deposit, "KZT", 4, 6)]
    public async Task Handle_Should_Returns_Wallet(WalletType type, string currencyValue,
        int balance, int? interestRate)
    {
        // ReSharper disable NullableWarningSuppressionIsUsed

        // Arrange
        var walletId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();
        var currencyValueObject = new CurrencyValueObject { Currency = currencyValue };

        var wallet = new WalletEntity(walletId, DateTime.UtcNow, null, null, false,
            ownerId, type, currencyValueObject, DateTime.UtcNow, null,
            interestRate, [], balance, Guid.NewGuid());

        // ReSharper disable SpecifyACultureInStringConversionExplicitly
        var responseDto = new WalletDto(walletId, DateTime.UtcNow, null, null, false,
            ownerId, type, currencyValueObject, DateTime.UtcNow, null,
            interestRate, [], balance);

        var mapperMock = new Mock<IMapper>();
        mapperMock
            .Setup(m => m.Map<WalletDto>(wallet))
            .Returns(responseDto);

        var walletRepositoryMock = new Mock<IWalletRepository>();

        walletRepositoryMock
            .Setup(r => r.Get(walletId))
            .Returns(Task.FromResult(wallet)!);

        var handler = new GetWalletQueryHandler(mapperMock.Object, walletRepositoryMock.Object);


        // Act
        var dto = await handler.Handle(new GetWalletQuery(walletId, ownerId), CancellationToken.None);

        // Assert
        Assert.Equal(responseDto.Id, dto.Id);
        Assert.Equal(responseDto.OwnerId, dto.OwnerId);
        Assert.Equal(responseDto.Currency, dto.Currency);
        Assert.Equal(responseDto.Type, dto.Type);
        Assert.Equal(responseDto.Balance, dto.Balance);
    }

    [Fact]
    public async Task Handle_Should_Returns_Forbid_Exception()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var mapperMock = new Mock<IMapper>();
        var walletRepositoryMock = new Mock<IWalletRepository>();

        walletRepositoryMock
            .Setup(r => r.Get(walletId))
            .Returns(Task.FromResult(new WalletEntity { OwnerId = ownerId })!);

        var handler = new GetWalletQueryHandler(mapperMock.Object, walletRepositoryMock.Object);
        var notOwnerId = Guid.NewGuid();

        // Act & Assert
        await Assert.ThrowsAsync<ForbiddenException>(() =>
            handler.Handle(new GetWalletQuery(walletId, notOwnerId), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Returns_NotFound_Exception()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var walletRepositoryMock = new Mock<IWalletRepository>();

        walletRepositoryMock
            .Setup(r => r.Get(walletId))
            .Returns(Task.FromResult<WalletEntity?>(null));

        var handler = new GetWalletQueryHandler(new Mock<IMapper>().Object, walletRepositoryMock.Object);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() =>
            handler.Handle(new GetWalletQuery(walletId, Guid.NewGuid()), CancellationToken.None));
    }
}