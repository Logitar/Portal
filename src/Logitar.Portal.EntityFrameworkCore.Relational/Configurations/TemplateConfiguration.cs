using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class TemplateConfiguration : AggregateConfiguration<TemplateEntity>, IEntityTypeConfiguration<TemplateEntity>
{
  public override void Configure(EntityTypeBuilder<TemplateEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PortalContext.Templates));
    builder.HasKey(x => x.TemplateId);

    builder.HasIndex(x => x.TenantId);
    builder.HasIndex(x => x.UniqueName);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Subject);
    builder.HasIndex(x => x.ContentType);
    builder.HasIndex(x => new { x.TenantId, x.UniqueNameNormalized }).IsUnique();

    builder.Property(x => x.TenantId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UniqueName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.UniqueNameNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Subject).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.ContentType).HasMaxLength(16);

    builder.HasOne(x => x.PasswordRecoveryInRealm).WithOne(x => x.PasswordRecoveryTemplate).OnDelete(DeleteBehavior.Restrict);
  }
}
