using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Configurations;

internal class TemplateConfiguration : AggregateConfiguration<TemplateEntity>, IEntityTypeConfiguration<TemplateEntity>
{
  public override void Configure(EntityTypeBuilder<TemplateEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.TemplateId);

    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => new { x.RealmId, x.UniqueNameNormalized }).IsUnique();

    builder.Property(x => x.UniqueName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Subject).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.ContentType).HasMaxLength(byte.MaxValue);

    builder.HasOne(x => x.Realm).WithMany(x => x.Templates).OnDelete(DeleteBehavior.Restrict);
  }
}
