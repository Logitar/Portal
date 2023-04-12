using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Configurations;

internal class SenderConfiguration : AggregateConfiguration<SenderEntity>, IEntityTypeConfiguration<SenderEntity>
{
  public override void Configure(EntityTypeBuilder<SenderEntity> builder)
  {
    base.Configure(builder);

    builder.HasKey(x => x.SenderId);

    builder.HasIndex(x => x.EmailAddress);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.Provider);
    builder.HasIndex(x => new { x.RealmId, x.IsDefault }).IsUnique().HasFilter($@"""{nameof(SenderEntity.IsDefault)}"" = true");

    builder.Property(x => x.EmailAddress).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Provider).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Settings).HasColumnType("jsonb");

    builder.HasOne(x => x.Realm).WithMany(x => x.Senders).OnDelete(DeleteBehavior.Restrict);
  }
}
