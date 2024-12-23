using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
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

    builder.HasIndex(x => x.UniqueKey);
    builder.HasIndex(x => new { x.TenantId, x.UniqueKeyNormalized }).IsUnique();
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Subject);
    builder.HasIndex(x => x.ContentType);

    builder.Property(x => x.TenantId).HasMaxLength(AggregateId.MaximumLength);
    builder.Property(x => x.UniqueKey).HasMaxLength(IdentifierValidator.MaximumLength);
    builder.Property(x => x.UniqueKeyNormalized).HasMaxLength(IdentifierValidator.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Subject).HasMaxLength(SubjectUnit.MaximumLength);
    builder.Property(x => x.ContentType).HasMaxLength(ContentTypeMaximumLength);
  }
}
