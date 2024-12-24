using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class TemplateConfiguration : AggregateConfiguration<TemplateEntity>, IEntityTypeConfiguration<TemplateEntity>
{
  public const int ContentTypeMaximumLength = 10; // NOTE(fpion): length of 'text/plain'

  public override void Configure(EntityTypeBuilder<TemplateEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PortalContext.Templates));
    builder.HasKey(x => x.TemplateId);

    builder.HasIndex(x => new { x.TenantId, x.EntityId }).IsUnique();
    builder.HasIndex(x => x.EntityId);
    builder.HasIndex(x => x.UniqueKey);
    builder.HasIndex(x => new { x.TenantId, x.UniqueKeyNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Subject);
    builder.HasIndex(x => x.ContentType);

    builder.Property(x => x.TenantId).HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.EntityId).HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.UniqueKey).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.UniqueKeyNormalized).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Subject).HasMaxLength(Subject.MaximumLength);
    builder.Property(x => x.ContentType).HasMaxLength(ContentTypeMaximumLength);
  }
}
