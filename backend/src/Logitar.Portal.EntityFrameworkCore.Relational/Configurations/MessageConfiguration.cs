using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
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

    builder.HasIndex(x => new { x.TenantId, x.EntityId }).IsUnique();
    builder.HasIndex(x => x.EntityId);
    builder.HasIndex(x => x.TenantId);
    builder.HasIndex(x => x.Subject);
    builder.HasIndex(x => x.RecipientCount);
    builder.HasIndex(x => x.IsDemo);
    builder.HasIndex(x => x.Status);

    builder.Property(x => x.TenantId).HasMaxLength(StreamId.MaximumLength);
    builder.Property(x => x.Subject).HasMaxLength(Subject.MaximumLength);
    builder.Property(x => x.BodyType).HasMaxLength(TemplateConfiguration.ContentTypeMaximumLength);
    builder.Property(x => x.SenderAddress).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.SenderPhoneNumber).HasMaxLength(Phone.NumberMaximumLength);
    builder.Property(x => x.SenderDisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.SenderProvider).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SenderProvider>());
    builder.Property(x => x.TemplateUniqueKey).HasMaxLength(Identifier.MaximumLength);
    builder.Property(x => x.TemplateDisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Locale).HasMaxLength(Locale.MaximumLength);
    builder.Property(x => x.Status).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<MessageStatus>());

    builder.HasOne(x => x.Sender).WithMany(x => x.Messages).OnDelete(DeleteBehavior.SetNull);
    builder.HasOne(x => x.Template).WithMany(x => x.Messages).OnDelete(DeleteBehavior.SetNull);
  }
}
