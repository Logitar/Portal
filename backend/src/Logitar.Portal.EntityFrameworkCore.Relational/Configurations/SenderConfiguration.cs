using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
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

    builder.HasIndex(x => new { x.TenantId, x.EntityId }).IsUnique();
    builder.HasIndex(x => x.EntityId);
    builder.HasIndex(x => new { x.TenantId, x.IsDefault });
    builder.HasIndex(x => x.EmailAddress);
    builder.HasIndex(x => x.PhoneNumber);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Provider);

    builder.Property(x => x.EmailAddress).HasMaxLength(Email.MaximumLength);
    builder.Property(x => x.PhoneNumber).HasMaxLength(Phone.NumberMaximumLength);
    builder.Property(x => x.DisplayName).HasMaxLength(DisplayName.MaximumLength);
    builder.Property(x => x.Provider).HasMaxLength(byte.MaxValue).HasConversion(new EnumToStringConverter<SenderProvider>());
  }
}
