using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class DictionaryConfiguration : AggregateConfiguration<DictionaryEntity>, IEntityTypeConfiguration<DictionaryEntity>
{
  public override void Configure(EntityTypeBuilder<DictionaryEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PortalContext.Dictionaries));
    builder.HasKey(x => x.DictionaryId);

    builder.HasIndex(x => x.TenantId);
    builder.HasIndex(x => new { x.TenantId, x.Locale }).IsUnique();

    builder.Ignore(x => x.Entries);

    builder.Property(x => x.TenantId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Locale).HasMaxLength(16);
    builder.Property(x => x.EntriesSerialized).HasColumnName(nameof(DictionaryEntity.EntriesSerialized));
  }
}
