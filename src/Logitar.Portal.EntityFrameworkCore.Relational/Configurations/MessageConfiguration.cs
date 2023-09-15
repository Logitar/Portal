using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class MessageConfiguration : AggregateConfiguration<MessageEntity>, IEntityTypeConfiguration<MessageEntity>
{
  public override void Configure(EntityTypeBuilder<MessageEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PortalContext.Messages));
    builder.HasKey(x => x.MessageId);

    builder.Ignore(x => x.RealmSummary);
    builder.Ignore(x => x.SenderSummary);
    builder.Ignore(x => x.TemplateSummary);
    builder.Ignore(x => x.Variables);
    builder.Ignore(x => x.Result);

    builder.Property(x => x.Subject).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.RealmSummarySerialized).HasColumnName(nameof(MessageEntity.RealmSummary));
    builder.Property(x => x.SenderSummarySerialized).HasColumnName(nameof(MessageEntity.SenderSummary));
    builder.Property(x => x.TemplateSummarySerialized).HasColumnName(nameof(MessageEntity.TemplateSummary));
    builder.Property(x => x.Locale).HasMaxLength(16);
    builder.Property(x => x.VariablesSerialized).HasColumnName(nameof(MessageEntity.Variables));
    builder.Property(x => x.ResultSerialized).HasColumnName(nameof(MessageEntity.Result));
    builder.Property(x => x.Status).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<MessageStatus>());

    builder.HasOne(x => x.Realm).WithMany(x => x.Messages).OnDelete(DeleteBehavior.SetNull);
    builder.HasOne(x => x.Sender).WithMany(x => x.Messages).OnDelete(DeleteBehavior.SetNull);
    builder.HasOne(x => x.Template).WithMany(x => x.Messages).OnDelete(DeleteBehavior.SetNull);
  }
}
