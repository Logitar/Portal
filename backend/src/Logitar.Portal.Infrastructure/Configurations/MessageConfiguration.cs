using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class MessageConfiguration : AggregateConfiguration<MessageEntity>, IEntityTypeConfiguration<MessageEntity>
  {
    public override void Configure(EntityTypeBuilder<MessageEntity> builder)
    {
      base.Configure(builder);

      builder.HasKey(x => x.MessageId);

      builder.HasIndex(x => x.Subject);
      builder.HasIndex(x => x.SenderAddress);
      builder.HasIndex(x => x.SenderDisplayName);
      builder.HasIndex(x => x.TemplateKey);
      builder.HasIndex(x => x.TemplateKeyNormalized);
      builder.HasIndex(x => x.TemplateDisplayName);
      builder.HasIndex(x => x.RealmId);
      builder.HasIndex(x => x.RealmAliasNormalized);
      builder.HasIndex(x => x.RealmAlias);
      builder.HasIndex(x => x.RealmDisplayName);
      builder.HasIndex(x => x.IsDemo);
      builder.HasIndex(x => x.HasErrors);
      builder.HasIndex(x => x.HasSucceeded);

      builder.Property(x => x.Subject).HasMaxLength(256);
      builder.Property(x => x.SenderId).HasMaxLength(256);
      builder.Property(x => x.SenderAddress).HasMaxLength(256);
      builder.Property(x => x.SenderDisplayName).HasMaxLength(256);
      builder.Property(x => x.TemplateId).HasMaxLength(256);
      builder.Property(x => x.TemplateKey).HasMaxLength(256);
      builder.Property(x => x.TemplateKeyNormalized).HasMaxLength(256);
      builder.Property(x => x.TemplateDisplayName).HasMaxLength(256);
      builder.Property(x => x.TemplateContentType).HasMaxLength(256);
      builder.Property(x => x.RealmId).HasMaxLength(256);
      builder.Property(x => x.RealmAlias).HasMaxLength(256);
      builder.Property(x => x.RealmAliasNormalized).HasMaxLength(256);
      builder.Property(x => x.RealmDisplayName).HasMaxLength(256);
      builder.Property(x => x.Locale).HasMaxLength(16);
      builder.Property(x => x.Variables).HasColumnType("jsonb");
      builder.Property(x => x.Errors).HasColumnType("jsonb");
      builder.Property(x => x.Result).HasColumnType("jsonb");
    }
  }
}
