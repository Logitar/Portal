using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Templates;
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

    builder.HasIndex(x => x.TenantId);
    builder.HasIndex(x => x.Subject);
    builder.HasIndex(x => x.RecipientCount);
    builder.HasIndex(x => x.IsDemo);
    builder.HasIndex(x => x.Status);

    builder.Ignore(x => x.Variables);
    builder.Ignore(x => x.ResultData);

    builder.Property(x => x.TenantId).HasMaxLength(AggregateId.MaximumLength);
    builder.Property(x => x.Subject).HasMaxLength(SubjectUnit.MaximumLength);
    builder.Property(x => x.BodyType).HasMaxLength(TemplateConfiguration.ContentTypeMaximumLength);
    builder.Property(x => x.SenderAddress).HasMaxLength(EmailUnit.MaximumLength);
    builder.Property(x => x.SenderPhoneNumber).HasMaxLength(PhoneUnit.NumberMaximumLength);
    builder.Property(x => x.SenderDisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.SenderProvider).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SenderProvider>());
    builder.Property(x => x.TemplateUniqueKey).HasMaxLength(IdentifierValidator.MaximumLength);
    builder.Property(x => x.TemplateDisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Locale).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.VariablesSerialized).HasColumnName(nameof(MessageEntity.Variables));
    builder.Property(x => x.Status).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<MessageStatus>());
    builder.Property(x => x.ResultDataSerialized).HasColumnName(nameof(MessageEntity.ResultData));

    builder.HasOne(x => x.Sender).WithMany(x => x.Messages).OnDelete(DeleteBehavior.SetNull);
    builder.HasOne(x => x.Template).WithMany(x => x.Messages).OnDelete(DeleteBehavior.SetNull);
  }
}
