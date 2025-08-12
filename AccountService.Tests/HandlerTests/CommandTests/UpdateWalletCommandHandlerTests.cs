using AccountService.Shared.Domain;
using AccountService.Shared.Exceptions;
using AccountService.Shared.Infrastructure;
using AccountService.Transactions.Domain;
using AccountService.Wallets.Domain;
using AccountService.Wallets.UpdateWallet;
using Moq;

namespace AccountService.Tests.HandlerTests.CommandTests;

public class UpdateWalletCommandHandlerTests
{
    [Fact]
    public async Task Handle_Should_Update_Wallet_When_Valid()
    {
        // Arrange
        Mock<MainDbContext> dbContextMock = new();
        var walletRepositoryMock = new Mock<IWalletRepository>();
        var handler = new UpdateWalletCommandHandler(walletRepositoryMock.Object, dbContextMock.Object);

        var walletId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var oldWallet = new WalletEntity(
            walletId, DateTime.UtcNow, null, null, false, ownerId,
            WalletType.Checking, new CurrencyValueObject { Currency = "USD" }, DateTime.UtcNow, null, null,
            [], 100, Guid.NewGuid());

        var entryMock = EntryMocking.MockEntityEntry(dbContextMock, oldWallet);
        dbContextMock.Setup(x => x.Entry(It.IsAny<WalletEntity>())).Returns(entryMock.Object);

        var command = CreateCommand(walletId, ownerId);

        walletRepositoryMock.Setup(r => r.Get(walletId))
            .ReturnsAsync(oldWallet);
        walletRepositoryMock.Setup(r => r.Update(It.IsAny<WalletEntity>()))
            .Returns(Task.CompletedTask);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        walletRepositoryMock.Verify(r => r.Update(It.Is<WalletEntity>(
            w => w.Id == walletId &&
                 w.Type == command.NewType &&
                 w.Currency.ToString() == command.NewCurrency.ToString() &&
                 w.InterestRate == command.NewInterestRate &&
                 w.Balance == command.NewBalance
        )), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_NotFound_When_Wallet_Does_Not_Exist()
    {
        // Arrange
        var walletRepositoryMock = new Mock<IWalletRepository>();
        var handler = new UpdateWalletCommandHandler(walletRepositoryMock.Object, new Mock<MainDbContext>().Object);
        var command = CreateCommand(Guid.NewGuid(), Guid.NewGuid());

        walletRepositoryMock.Setup(r => r.Get(command.Id))
            .ReturnsAsync((WalletEntity?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequest_When_Wallet_Is_Deleted()
    {
        // Arrange
        var walletRepositoryMock = new Mock<IWalletRepository>();
        var handler = new UpdateWalletCommandHandler(walletRepositoryMock.Object, new Mock<MainDbContext>().Object);
        var walletId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var wallet = new WalletEntity(walletId, DateTime.UtcNow, null, null, true, ownerId,
            WalletType.Checking, new CurrencyValueObject { Currency = "USD" }, DateTime.UtcNow, null, null,
            [], 1000, Guid.NewGuid());
        var command = CreateCommand(walletId, ownerId);

        walletRepositoryMock.Setup(r => r.Get(wallet.Id))
            .ReturnsAsync(wallet);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequest_When_Wallet_Has_Transactions()
    {
        // Arrange
        var walletRepositoryMock = new Mock<IWalletRepository>();
        var handler = new UpdateWalletCommandHandler(walletRepositoryMock.Object, new Mock<MainDbContext>().Object);
        var walletId = Guid.NewGuid();
        var ownerId = Guid.NewGuid();

        var wallet = new WalletEntity(walletId, DateTime.UtcNow, null, null, false, ownerId,
            WalletType.Checking, new CurrencyValueObject { Currency = "USD" }, DateTime.UtcNow, null, null,
            [
                It.IsAny<TransactionEntity>(), It.IsAny<TransactionEntity>()
            ], 1000, Guid.NewGuid());

        var command = CreateCommand(walletId, ownerId);

        walletRepositoryMock.Setup(r => r.Get(wallet.Id))
            .ReturnsAsync(wallet);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => handler.Handle(command, CancellationToken.None));
    }

    private static UpdateWalletCommand CreateCommand(Guid walletId, Guid ownerId)
    {
        return new UpdateWalletCommand
        {
            Id = walletId,
            OwnerId = ownerId,
            NewBalance = 90,
            NewCurrency = new CurrencyValueObject { Currency = "RUB" },
            NewInterestRate = 3,
            NewType = WalletType.Deposit
        };
    }
}