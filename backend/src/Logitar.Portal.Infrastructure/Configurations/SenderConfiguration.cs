using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class SenderConfiguration : AggregateConfiguration<SenderEntity>, IEntityTypeConfiguration<SenderEntity>
  {
    public override void Configure(EntityTypeBuilder<SenderEntity> builder)
    {
      base.Configure(builder);

      builder.HasKey(x => x.SenderId);

      builder.HasOne(x => x.Realm).WithMany(x => x.Senders).OnDelete(DeleteBehavior.Restrict);

      builder.HasIndex(x => x.IsDefault).HasFilter(@"""IsDefault"" = true");
      builder.HasIndex(x => x.EmailAddress);
      builder.HasIndex(x => x.DisplayName);
      builder.HasIndex(x => x.Provider);

      builder.Property(x => x.EmailAddress).HasMaxLength(255);
      builder.Property(x => x.DisplayName).HasMaxLength(255);
      builder.Property(x => x.Settings).HasColumnType("jsonb");
    }
  }
}
