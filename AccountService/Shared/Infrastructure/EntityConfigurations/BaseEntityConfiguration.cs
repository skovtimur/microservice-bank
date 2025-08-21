using AccountService.Shared.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AccountService.Shared.Infrastructure.EntityConfigurations;

public class BaseEntityConfiguration : IEntityTypeConfiguration<BaseEntity>
{
    public static readonly ValueConverter<DateTime, DateTime> DateTimeConverter = new(
        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
        v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    public void Configure(EntityTypeBuilder<BaseEntity> builder)
    {
        builder.Property(x => x.Id).HasColumnName("id").IsRequired();
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CreatedAtUtc).HasConversion(DateTimeConverter).HasColumnName("created_at_utc").IsRequired();
        builder.Property(x => x.UpdatedAtUtc).HasConversion(DateTimeConverter).HasColumnName("updated_at_utc");
        builder.Property(x => x.DeletedAtUtc).HasConversion(DateTimeConverter).HasColumnName("deleted_at_utc");
        builder.Property(x => x.IsDeleted).HasColumnName("is_deleted").IsRequired().HasDefaultValue(false);
        builder.Property(x => x.EntityVersion).HasColumnName("entity_version").IsRequired()
            .IsConcurrencyToken();

        builder.UseTpcMappingStrategy();
    }
}