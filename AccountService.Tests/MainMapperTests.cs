using AccountService.Shared.Domain;
using AccountService.Shared.Mapper;
using AccountService.Transactions.CreateTransaction;
using AccountService.Transactions.Domain;
using AccountService.Wallets.CreateWallet;
using AccountService.Wallets.Domain;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AccountService.Tests;

public class MainMapperTests
{
    private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
    {
        builder.SetMinimumLevel(LogLevel.Trace)
            .AddConsole();
    });

    private readonly IMapper _mapper;

    public MainMapperTests()
    {
        var config = new MapperConfiguration(cfg => { cfg.AddProfile<MainMapper>(); }, _loggerFactory);
        _mapper = config.CreateMapper();
    }


    [Fact]
    public void CreateWalletCommand_To_WalletEntity_MapsCorrectly()
    {
        var command = new CreateWalletCommand
        {
            OwnerId = Guid.NewGuid(),
            Type = WalletType.Checking,
            Currency = new CurrencyValueObject { Currency = "USD" },
            Balance = 1000,
            InterestRate = null,
            ClosedAtUtc = null
        };

        var entity = _mapper.Map<WalletEntity>(command);

        Assert.Equal(command.OwnerId, entity.OwnerId);
        Assert.Equal(command.Type, entity.Type);
        Assert.Equal(command.Currency.Currency, entity.Currency.Currency);
        Assert.Equal(command.Balance, entity.Balance);
        Assert.Equal(command.InterestRate, entity.InterestRate);
        Assert.Equal(command.ClosedAtUtc, entity.ClosedAtUtc);
    }

    [Fact]
    public void TransactionCreateCommand_To_TransactionEntity_MapsCorrectly()
    {
        var command = new TransactionCreateCommand
        {
            OwnerId = Guid.NewGuid(),
            AccountId = Guid.NewGuid(),
            CounterpartyAccountId = Guid.NewGuid(),
            Sum = 120,
            TransactionType = TransactionType.Credit,
            Currency = new CurrencyValueObject { Currency = "EUR" },
            Description = new DescriptionValueObject { Description = "Test description" }
        };

        var entity = _mapper.Map<TransactionEntity>(command);

        Assert.Equal(command.OwnerId, entity.OwnerId);
        Assert.Equal(command.AccountId, entity.AccountId);
        Assert.Equal(command.Sum, entity.Sum);
        Assert.Equal(command.Currency.Currency, entity.Currency.Currency);
        Assert.Equal(command.TransactionType, entity.TransactionType);
        Assert.Equal(command.Description, entity.Description);
        Assert.Equal(command.CounterpartyAccountId, entity.CounterpartyAccountId);
    }

    [Fact]
    public void WalletEntity_To_WalletDto_ConvertsDatesAndNestedTransactions()
    {
        var ownerId = Guid.NewGuid();
        var accountId = Guid.NewGuid();

        var walletEntity = new WalletEntity
            (
                accountId,
                DateTime.UtcNow.AddDays(-10),
                DateTime.UtcNow.AddDays(-5),
                null,
                false,
                ownerId,
                WalletType.Checking,
                new CurrencyValueObject { Currency = "USD" },
                DateTime.UtcNow.AddDays(-10),
                null,
                null,
                [
                    new TransactionEntity(
                        accountId: accountId,
                        ownerId: ownerId,
                        counterpartyAccountId: null,
                        sum: 200,
                        currency: new CurrencyValueObject { Currency = "USD" },
                        description: new DescriptionValueObject { Description = "Test description" },
                        transactionType: TransactionType.Debit
                    )
                ],
                500,
                Guid.NewGuid()
            );

        var dto = _mapper.Map<WalletDto>(walletEntity);

        Assert.Equal(walletEntity.Id, dto.Id);
        Assert.Equal(walletEntity.OwnerId, dto.OwnerId);
        Assert.Equal(walletEntity.Type.ToString(), dto.Type);
        Assert.Equal(walletEntity.Currency.Currency, dto.Currency);
        Assert.Equal(walletEntity.Transactions.Count, dto.Transactions.Count);
        Assert.NotNull(dto.CreatedAtUtc);
        Assert.NotNull(dto.OpenedAtUtc);

        for (var i = 0; i < dto.Transactions.Count; i++)
        {
            Assert.Equal(walletEntity.Transactions[i].TransactionType.ToString(), dto.Transactions[i].TransactionType);
            Assert.Equal(walletEntity.Transactions[i].Currency.Currency, dto.Transactions[i].Currency);
            Assert.Equal(walletEntity.Transactions[i].OwnerId, dto.Transactions[i].OwnerId);
            Assert.Equal(walletEntity.Transactions[i].Id, dto.Transactions[i].Id);
            Assert.Equal(walletEntity.Transactions[i].CounterpartyAccountId, dto.Transactions[i].CounterpartyAccountId);
            Assert.Equal(walletEntity.Transactions[i].Sum, dto.Transactions[i].Sum);
            Assert.False(walletEntity.Transactions[i].IsDeleted);
            Assert.False(dto.Transactions[i].IsDeleted);
            Assert.Equal(walletEntity.Transactions[i].Description.ToString(), dto.Transactions[i].Description);
            Assert.NotNull(dto.Transactions[i].CreatedAtUtc);
        }
    }

    [Fact]
    public void TransactionEntity_To_TransactionDto_ConvertsEnumsAndDates()
    {
        var transactionEntity = new TransactionEntity(
            accountId: Guid.NewGuid(),
            ownerId: Guid.NewGuid(),
            counterpartyAccountId: null,
            sum: 150,
            currency: new CurrencyValueObject { Currency = "USD" },
            description: new DescriptionValueObject { Description = "Test description" },
            transactionType: TransactionType.Debit
        );

        var dto = _mapper.Map<TransactionDto>(transactionEntity);

        Assert.Equal(transactionEntity.TransactionType.ToString(), dto.TransactionType);
        Assert.Equal(transactionEntity.Currency.Currency, dto.Currency);
        Assert.Equal(transactionEntity.OwnerId, dto.OwnerId);
        Assert.Equal(transactionEntity.Id, dto.Id);
        Assert.Equal(transactionEntity.CounterpartyAccountId, dto.CounterpartyAccountId);
        Assert.Equal(transactionEntity.Sum, dto.Sum);
        Assert.False(transactionEntity.IsDeleted);
        Assert.False(dto.IsDeleted);
        Assert.Equal(transactionEntity.Description.ToString(), dto.Description);
        Assert.NotNull(dto.CreatedAtUtc);
    }
}