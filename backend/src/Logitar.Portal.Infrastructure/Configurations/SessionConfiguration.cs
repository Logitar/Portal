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

      builder.HasOne(x => x.User).WithMany(x => x.Sessions).OnDelete(DeleteBehavior.NoAction);

      builder.HasIndex(x => x.IsActive);
      builder.HasIndex(x => x.IsPersistent);

      builder.Property(x => x.AdditionalInformation).HasColumnType("jsonb");
      builder.Property(x => x.IpAddress).HasMaxLength(40);
      builder.Property(x => x.SignedOutBy).HasMaxLength(256);
    }
  }
}
