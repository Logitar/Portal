using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Portal.Core.Emails.Messages;

namespace Portal.Infrastructure.Configurations
{
  internal class MessageConfiguration : AggregateConfiguration<Message>, IEntityTypeConfiguration<Message>
  {
    public override void Configure(EntityTypeBuilder<Message> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => x.HasErrors);
      builder.HasIndex(x => x.RealmAlias);
      builder.HasIndex(x => x.RealmId);
      builder.HasIndex(x => x.RealmName);
      builder.HasIndex(x => x.SenderAddress);
      builder.HasIndex(x => x.SenderDisplayName);
      builder.HasIndex(x => x.SenderId);
      builder.HasIndex(x => x.SenderProvider);
      builder.HasIndex(x => x.Subject);
      builder.HasIndex(x => x.Succeeded);
      builder.HasIndex(x => x.TemplateContentType);
      builder.HasIndex(x => x.TemplateDisplayName);
      builder.HasIndex(x => x.TemplateId);
      builder.HasIndex(x => x.TemplateKey);

      builder.Ignore(x => x.Errors);
      builder.Ignore(x => x.Recipients);
      builder.Ignore(x => x.Result);
      builder.Ignore(x => x.Variables);

      builder.Property(x => x.ErrorsSerialized).HasColumnName(nameof(Message.Errors)).HasColumnType("jsonb");
      builder.Property(x => x.RealmAlias).HasMaxLength(256);
      builder.Property(x => x.RealmName).HasMaxLength(256);
      builder.Property(x => x.RecipientsSerialized).HasColumnName(nameof(Message.Recipients)).HasColumnType("jsonb");
      builder.Property(x => x.ResultSerialized).HasColumnName(nameof(Message.Result)).HasColumnType("jsonb");
      builder.Property(x => x.SenderAddress).HasMaxLength(256);
      builder.Property(x => x.SenderDisplayName).HasMaxLength(256);
      builder.Property(x => x.Subject).HasMaxLength(256);
      builder.Property(x => x.TemplateContentType).HasMaxLength(256);
      builder.Property(x => x.TemplateDisplayName).HasMaxLength(256);
      builder.Property(x => x.TemplateKey).HasMaxLength(256);
      builder.Property(x => x.VariablesSerialized).HasColumnName(nameof(Message.Variables)).HasColumnType("jsonb");
    }
  }
}
