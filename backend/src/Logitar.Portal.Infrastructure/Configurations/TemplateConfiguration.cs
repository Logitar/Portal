using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class TemplateConfiguration : AggregateConfiguration<TemplateEntity>, IEntityTypeConfiguration<TemplateEntity>
  {
    public override void Configure(EntityTypeBuilder<TemplateEntity> builder)
    {
      base.Configure(builder);

      builder.HasKey(x => x.TemplateId);

      builder.HasOne(x => x.Realm).WithMany(x => x.Templates).OnDelete(DeleteBehavior.Restrict);

      builder.HasIndex(x => x.Key);
      builder.HasIndex(x => x.DisplayName);
      builder.HasIndex(x => new { x.RealmId, x.KeyNormalized }).IsUnique();

      builder.Property(x => x.Key).HasMaxLength(255);
      builder.Property(x => x.KeyNormalized).HasMaxLength(255);
      builder.Property(x => x.DisplayName).HasMaxLength(255);

      builder.Property(x => x.Subject).HasMaxLength(255);
      builder.Property(x => x.ContentType).HasMaxLength(255);
    }
  }
}
