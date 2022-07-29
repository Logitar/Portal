using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portal.Core.Emails.Templates;
using System.Net.Mime;

namespace Portal.Infrastructure.Configurations
{
  internal class TemplateConfiguration : AggregateConfiguration<Template>, IEntityTypeConfiguration<Template>
  {
    public override void Configure(EntityTypeBuilder<Template> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => x.DisplayName);
      builder.HasIndex(x => x.Key);
      builder.HasIndex(x => new { x.RealmSid, x.KeyNormalized }).IsUnique();

      builder.Property(x => x.ContentType).HasMaxLength(256).HasDefaultValue(MediaTypeNames.Text.Plain);
      builder.Property(x => x.DisplayName).HasMaxLength(256);
      builder.Property(x => x.Key).HasMaxLength(256);
      builder.Property(x => x.KeyNormalized).HasMaxLength(256);
    }
  }
}
