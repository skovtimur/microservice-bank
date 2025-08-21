using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AccountService.Shared.Infrastructure;

// ReSharper disable once UnusedType.Global
// Нужен для того чтобы делать миграции и Entity framework не орал почему не может api подключится к rabbitmq
public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<MainDbContext>
{
    public MainDbContext CreateDbContext(string[] args)
    {
        // Загружаем конфиг
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json")
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<MainDbContext>();

        var connectionString = configuration.GetConnectionString("account_service_db");
        optionsBuilder.UseNpgsql(connectionString);

        return new MainDbContext(optionsBuilder.Options);
    }
}