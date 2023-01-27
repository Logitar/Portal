using Logitar.Portal.Domain.Dictionaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class DictionaryConfiguration : AggregateConfiguration<Dictionary>, IEntityTypeConfiguration<Dictionary>
  {
    public override void Configure(EntityTypeBuilder<Dictionary> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => new { x.RealmSid, x.Locale }).IsUnique();

      builder.HasOne(x => x.Realm).WithMany(x => x.Dictionaries).OnDelete(DeleteBehavior.NoAction);

      builder.Ignore(x => x.Entries);

      builder.Property(x => x.EntriesSerialized).HasColumnName(nameof(Dictionary.Entries)).HasColumnType("jsonb");
      builder.Property(x => x.Locale).HasMaxLength(16);
    }
  }
}
