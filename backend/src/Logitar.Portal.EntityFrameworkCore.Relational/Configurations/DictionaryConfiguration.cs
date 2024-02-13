using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
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

    builder.HasIndex(x => x.Locale);
    builder.HasIndex(x => new { x.TenantId, x.Locale }).IsUnique();
    builder.HasIndex(x => x.EntryCount);

    builder.Ignore(x => x.Entries);

    builder.Property(x => x.TenantId).HasMaxLength(AggregateId.MaximumLength);
    builder.Property(x => x.Locale).HasMaxLength(LocaleUnit.MaximumLength);
    builder.Property(x => x.EntriesSerialized).HasColumnName(nameof(DictionaryEntity.Entries));
  }
}
