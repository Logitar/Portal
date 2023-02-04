using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class DictionaryConfiguration : AggregateConfiguration<DictionaryEntity>, IEntityTypeConfiguration<DictionaryEntity>
  {
    public override void Configure(EntityTypeBuilder<DictionaryEntity> builder)
    {
      base.Configure(builder);

      builder.HasKey(x => x.DictionaryId);

      builder.HasOne(x => x.Realm).WithMany(x => x.Dictionaries).OnDelete(DeleteBehavior.Restrict);

      builder.HasIndex(x => new { x.RealmId, x.Locale }).IsUnique();

      builder.Property(x => x.Locale).HasMaxLength(16);
      builder.Property(x => x.Entries).HasColumnType("jsonb");
    }
  }
}
