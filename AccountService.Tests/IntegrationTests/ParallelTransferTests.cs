using System.Net;
using AccountService.Features.Transactions.Api.Requests;
using AccountService.Features.Wallets.Domain;
using AccountService.Shared.Domain;
using AccountService.Shared.Infrastructure;
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
    }

    private readonly MainDbContext _dbContext;
    private readonly Random _random = new();
    private readonly HttpClient _httpClient;

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

        var accountId = Guid.NewGuid();
        await _dbContext.Wallets.AddAsync(new WalletEntity(accountId, DateTime.UtcNow, null, null, false,
            AuthTestHandler.UserId, WalletType.Checking, new CurrencyValueObject { Currency = "USD" }, DateTime.UtcNow,
            null,
            null, [], accountBalance, Guid.NewGuid()));

        var counterpartyId = Guid.NewGuid();
        await _dbContext.Wallets.AddAsync(new WalletEntity(counterpartyId, DateTime.UtcNow, null, null, false,
            Guid.NewGuid(), WalletType.Deposit, new CurrencyValueObject { Currency = "USD" }, DateTime.UtcNow, null,
            2, [], counterpartyBalance, Guid.NewGuid()));

        await _dbContext.SaveChangesAsync();

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