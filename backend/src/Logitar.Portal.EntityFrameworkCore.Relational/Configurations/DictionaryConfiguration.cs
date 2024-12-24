using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
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

    builder.HasIndex(x => new { x.TenantId, x.EntityId }).IsUnique();
    builder.HasIndex(x => x.EntityId);
    builder.HasIndex(x => x.Locale);
    builder.HasIndex(x => new { x.TenantId, x.LocaleNormalized }).IsUnique();
    builder.HasIndex(x => x.EntryCount);

    builder.Property(x => x.Locale).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.LocaleNormalized).HasMaxLength(Locale.MaximumLength);
  }
}
