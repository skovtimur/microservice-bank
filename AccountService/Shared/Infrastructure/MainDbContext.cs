using System.Runtime.CompilerServices;
using AccountService.Features.Transactions.Domain;
using AccountService.Features.Wallets.Domain;
using AccountService.Shared.Infrastructure.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Shared.Infrastructure;

public class MainDbContext : DbContext
{
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
    public DbSet<WalletEntity> Wallets { get; set; }
    public DbSet<TransactionEntity> Transactions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BaseEntityConfiguration());
        modelBuilder.ApplyConfiguration(new WalletEntityConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionEntityConfiguration());
    }

    public static readonly ILoggerFactory GetLoggerFactory
        = LoggerFactory.Create(builder => { builder.AddConsole(); });
}
public static class MyModuleInitializer
{
    [ModuleInitializer]
    public static void Initialize()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }
}