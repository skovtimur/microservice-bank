using AccountService.Shared.Infrastructure.EntityConfigurations;
using AccountService.Transactions.Domain;
using AccountService.Wallets.Domain;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Shared.Infrastructure;

public class MainDbContext : DbContext
{
    public MainDbContext(DbContextOptions options) : base(options)
    {
    }

    public MainDbContext()
    {
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