using System.Net;
using AccountService.Shared.Domain;
using AccountService.Shared.Infrastructure;
using AccountService.Transactions.Api.Requests;
using AccountService.Wallets.CreateWallet;
using AccountService.Wallets.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccountService.Tests.IntegrationTests;

public class ParallelTransferTests : IClassFixture<IntegrationTestWebAppFactory>
{
    public ParallelTransferTests(IntegrationTestWebAppFactory factory)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var scope = factory.Services.CreateScope();
        _httpClient = factory.CreateClient();

        _dbContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
        _mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
    }

    private readonly MainDbContext _dbContext;
    private readonly Random _random = new();
    private readonly HttpClient _httpClient;
    private readonly IMediator _mediator;

    [Theory]
    [InlineData(50)]
    public async Task TransferMoney_Should_Run_Many_Times(int numberOfTransfer)
    {
        // ReSharper disable SpecifyACultureInStringConversionExplicitly
        if (numberOfTransfer <= 0)
            throw new ArgumentException("Must be greater than 0", nameof(numberOfTransfer));

        const decimal accountBalance = 1000000;
        const decimal counterpartyBalance = 900;
        const decimal commonBalance = accountBalance + counterpartyBalance;

        var accountId = await _mediator.Send(new CreateWalletCommand
        {
            OwnerId = AuthTestHandler.UserId,
            Type = WalletType.Checking,
            Currency = new CurrencyValueObject { Currency = "USD" },
            InterestRate = null,
            Balance = accountBalance,
            ClosedAtUtc = null
        });
        var counterpartyId = await _mediator.Send(new CreateWalletCommand
        {
            OwnerId = AuthTestHandler.UserId,
            Type = WalletType.Deposit,
            Currency = new CurrencyValueObject { Currency = "USD" },
            InterestRate = 2,
            Balance = counterpartyBalance,
            ClosedAtUtc = null
        });

        // Act
        var sum = _random.Next(0, 1000);
        var tasks = Enumerable.Range(0, numberOfTransfer).Select(i => Task.Run(async () =>
        {
            var request = new TransferMoneyRequest
            {
                AccountId = accountId,
                TransferToCounterpartyAccountId = counterpartyId,
                Sum = sum,
                IsoCurrencyCode = "USD",
                Description = $"Transfer test {i + 1}"
            };

            var content = new MultipartFormDataContent
            {
                { new StringContent(request.AccountId.ToString()), nameof(request.AccountId) },
                {
                    new StringContent(request.TransferToCounterpartyAccountId.ToString()),
                    nameof(request.TransferToCounterpartyAccountId)
                },
                { new StringContent(request.Sum.ToString()), nameof(request.Sum) },
                { new StringContent(request.IsoCurrencyCode), nameof(request.IsoCurrencyCode) },
                { new StringContent(request.Description), nameof(request.Description) }
            };
            var response = await _httpClient.PostAsync("/api/transactions/transfer-money", content);
            return response;
        }));
        var results = await Task.WhenAll(tasks); // ждём все таски

        // Assert
        Assert.Equal(numberOfTransfer, results.Length);
        
        foreach (var result in results)
        {
            Assert.True(result.StatusCode is HttpStatusCode.Conflict or HttpStatusCode.Created);
        }
        var accountFromDb = await _dbContext.Wallets
            .SingleOrDefaultAsync(x => x.Id == accountId);

        var counterpartyAccountFromDb = await _dbContext.Wallets
            .SingleOrDefaultAsync(x => x.Id == counterpartyId);

        Assert.NotNull(accountFromDb);
        Assert.NotNull(counterpartyAccountFromDb);
        Assert.Equal(commonBalance, accountFromDb.Balance + counterpartyAccountFromDb.Balance);
    }
}