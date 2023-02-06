using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class SessionConfiguration : AggregateConfiguration<SessionEntity>, IEntityTypeConfiguration<SessionEntity>
  {
    public override void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
      base.Configure(builder);

      builder.HasKey(x => x.SessionId);

      builder.HasOne(x => x.User).WithMany(x => x.Sessions).OnDelete(DeleteBehavior.Restrict);

      builder.HasIndex(x => x.IsPersistent);
      builder.HasIndex(x => x.SignedOutById);
      builder.HasIndex(x => x.SignedOutOn);
      builder.HasIndex(x => x.IsActive);
      builder.HasIndex(x => x.IpAddress);

      builder.Property(x => x.SignedOutById).HasMaxLength(255);
      builder.Property(x => x.SignedOutBy).HasColumnType("jsonb");
      builder.Property(x => x.IpAddress).HasMaxLength(64);
    }
  }
}
