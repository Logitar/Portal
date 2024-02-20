using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Identity.EntityFrameworkCore.Relational.Configurations;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class SenderConfiguration : AggregateConfiguration<SenderEntity>, IEntityTypeConfiguration<SenderEntity>
{
  public override void Configure(EntityTypeBuilder<SenderEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PortalContext.Senders));
    builder.HasKey(x => x.SenderId);

    builder.HasIndex(x => new { x.TenantId, x.IsDefault });
    builder.HasIndex(x => x.EmailAddress);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Provider);

    builder.Ignore(x => x.Settings);

    builder.Property(x => x.TenantId).HasMaxLength(AggregateId.MaximumLength);
    builder.Property(x => x.EmailAddress).HasMaxLength(EmailUnit.MaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayNameUnit.MaximumLength);
    builder.Property(x => x.Provider).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SenderProvider>());
    builder.Property(x => x.SettingsSerialized).HasColumnName(nameof(SenderEntity.Settings));
  }
}
