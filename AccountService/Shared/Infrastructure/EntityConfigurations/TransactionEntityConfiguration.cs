using AccountService.Features.Transactions.Domain;
using AccountService.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AccountService.Shared.Infrastructure.EntityConfigurations;

public class TransactionEntityConfiguration : IEntityTypeConfiguration<TransactionEntity>
{
    public void Configure(EntityTypeBuilder<TransactionEntity> builder)
    {
        builder.ToTable("transactions");
        builder.Property(x => x.AccountId).HasColumnName("account_id").IsRequired();
        builder.Property(x => x.OwnerId).HasColumnName("owner_id").IsRequired();
        builder.Property(x => x.CounterpartyAccountId).HasColumnName("counterparty_account_id");

        builder.Property(x => x.TransactionType).HasColumnName("transaction_type").IsRequired();
        builder.Property(x => x.Sum).HasColumnName("sum").IsRequired();

        builder.Property(x => x.Currency)
            .HasConversion(x => x.Currency, x => new CurrencyValueObject { Currency = x }).HasMaxLength(3)
            .HasColumnName("currency").IsRequired();

        builder.Property(x => x.Description)
            .HasConversion(x => x.Description, x => new DescriptionValueObject { Description = x })
            .HasMaxLength(DescriptionValidator.MaxDescriptionLength)
            .HasColumnName("description").IsRequired();

        builder.HasIndex(p => new
        {
            p.AccountId,
            p.CreatedAtUtc
        }).HasDatabaseName("IX_transactions_account_id_and_created_at_utc");

        // Если будут проблемы с Gist то нужно просто добавить в миграцию migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");
        builder.HasIndex(p => p.CreatedAtUtc)
            .HasMethod("GIST").HasDatabaseName("IX_transactions_created_at_utc_gist");
    }
}