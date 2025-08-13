using AccountService.Shared.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.PostgreSql;

namespace AccountService.Tests.IntegrationTests;

// ReSharper disable once ClassNeverInstantiated.Global
// не вижу делать смысл abstract как говорит Resharper
public class IntegrationTestWebAppFactory
    : Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactory<Program>,
        IAsyncLifetime
{
    private const string DatabaseName = "test_account_db";

    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase(DatabaseName)
        .WithUsername("postgres")
        .WithPassword("nigPostgres_Pas5432")
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            // ReSharper disable once NullableWarningSuppressionIsUsed
            config.AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:hangfire-db"] =
                    _postgresContainer.GetConnectionString().Replace(DatabaseName, "hangfire_db"),
                ["ConnectionStrings:account-service-db"] = _postgresContainer.GetConnectionString()
            }!);
        });

        builder.ConfigureTestServices(x =>
        {
            // Remove all existing auth registrations
            x.RemoveAll<IAuthenticationSchemeProvider>();
            x.RemoveAll<IAuthenticationHandlerProvider>();
            x.RemoveAll<IConfigureOptions<AuthenticationOptions>>();
            x.RemoveAll<IConfigureOptions<JwtBearerOptions>>();

            // Add only the test scheme
            x.AddAuthentication(AuthTestHandler.TestScheme)
                .AddScheme<AuthenticationSchemeOptions, AuthTestHandler>(
                    AuthTestHandler.TestScheme, _ => { });

            // Make it the default for everything
            x.PostConfigure<AuthenticationOptions>(options =>
            {
                options.DefaultAuthenticateScheme = AuthTestHandler.TestScheme;
                options.DefaultChallengeScheme = AuthTestHandler.TestScheme;
            });

            var dbDescriptor = x.SingleOrDefault(y => y.ServiceType == typeof(DbContextOptions<MainDbContext>));

            if (dbDescriptor != null)
                x.Remove(dbDescriptor);

            x.AddDbContext<MainDbContext>(opt =>
            {
                opt.UseNpgsql(_postgresContainer.GetConnectionString());
                opt.UseLoggerFactory(MainDbContext.GetLoggerFactory);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _postgresContainer.StopAsync();
    }
}