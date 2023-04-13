using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Configurations;

internal class DictionaryConfiguration : AggregateConfiguration<DictionaryEntity>, IEntityTypeConfiguration<DictionaryEntity>
{
  public override void Configure(EntityTypeBuilder<DictionaryEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.DictionaryId);

    builder.HasIndex(x => x.EntryCount);
    builder.HasIndex(x => x.Locale);
    builder.HasIndex(x => new { x.RealmId, x.Locale }).IsUnique();

    builder.Property(x => x.Locale).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Entries).HasColumnType("jsonb");

    builder.HasOne(x => x.Realm).WithMany(x => x.Dictionaries).OnDelete(DeleteBehavior.Restrict);
  }
}
