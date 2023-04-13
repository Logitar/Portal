using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Configurations;

internal class MessageConfiguration : AggregateConfiguration<MessageEntity>, IEntityTypeConfiguration<MessageEntity>
{
  public override void Configure(EntityTypeBuilder<MessageEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.MessageId);

    builder.HasIndex(x => x.IsDemo);
    builder.HasIndex(x => x.Subject);
    builder.HasIndex(x => x.RealmId);
    builder.HasIndex(x => x.RealmUniqueName);
    builder.HasIndex(x => x.RealmDisplayName);
    builder.HasIndex(x => x.SenderId);
    builder.HasIndex(x => x.SenderEmailAddress);
    builder.HasIndex(x => x.SenderDisplayName);
    builder.HasIndex(x => x.TemplateId);
    builder.HasIndex(x => x.TemplateUniqueName);
    builder.HasIndex(x => x.TemplateDisplayName);
    builder.HasIndex(x => x.HasErrors);
    builder.HasIndex(x => x.Succeeded);

    builder.Property(x => x.Subject).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Recipients).HasColumnType("jsonb");
    builder.Property(x => x.RealmUniqueName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.RealmDisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.SenderProvider).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.SenderEmailAddress).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.SenderDisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.TemplateUniqueName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.TemplateDisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.TemplateContentType).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Locale).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Variables).HasColumnType("jsonb");
    builder.Property(x => x.Errors).HasColumnType("jsonb");
    builder.Property(x => x.Result).HasColumnType("jsonb");
  }
}
