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

    builder.HasIndex(x => x.TenantId);
    builder.HasIndex(x => x.EmailAddress);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Provider);
    builder.HasIndex(x => new { x.TenantId, x.IsDefault }).IsUnique().HasFilter($@"""{nameof(SenderEntity.IsDefault)}"" = true");

    builder.Ignore(x => x.Settings);

    builder.Property(x => x.TenantId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.EmailAddress).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Provider).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<ProviderType>());
    builder.Property(x => x.SettingsSerialized).HasColumnName(nameof(SenderEntity.Settings));

    builder.HasOne(x => x.PasswordRecoveryInRealm).WithOne(x => x.PasswordRecoverySender).OnDelete(DeleteBehavior.Restrict);
  }
}
