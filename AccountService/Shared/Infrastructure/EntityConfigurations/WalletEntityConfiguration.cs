using AccountService.Shared.Domain;
using AccountService.Wallets.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.Shared.Infrastructure.EntityConfigurations;

public class WalletEntityConfiguration : IEntityTypeConfiguration<WalletEntity>
{
    public void Configure(EntityTypeBuilder<WalletEntity> builder)
    {
        builder.ToTable("wallets");

        builder.Property(x => x.OwnerId).IsRequired().HasColumnName("owner_id");
        builder.Property(x => x.Type).IsRequired().HasColumnName("type");

        builder.Property(x => x.Balance).IsRequired().HasColumnName("balance");
        builder.Property(x => x.InterestRate).HasMaxLength(100).HasColumnName("interest_rate");

        builder.Property(x => x.OpenedAtUtc).IsRequired().HasColumnName("opened_at_utc");
        builder.Property(x => x.ClosedAtUtc).HasColumnName("closed_at_utc");

        builder.HasMany(x => x.Transactions).WithOne()
            .HasForeignKey(x => x.AccountId)
            .HasConstraintName("ForeignKey_AccountId");

        builder.Property(x => x.Currency)
            .HasConversion(x => x.Currency, x => new CurrencyValueObject { Currency = x }).HasMaxLength(3)
            .HasColumnName("currency").IsRequired();

        builder.HasIndex(p => p.OwnerId).HasDatabaseName("IX_wallets_owner_id");
    }
}