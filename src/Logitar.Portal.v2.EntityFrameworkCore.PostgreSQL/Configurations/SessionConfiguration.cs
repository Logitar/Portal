using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Configurations
{
  internal class SessionConfiguration : AggregateConfiguration<SessionEntity>, IEntityTypeConfiguration<SessionEntity>
  {
    public override void Configure(EntityTypeBuilder<SessionEntity> builder)
    {
      base.Configure(builder);

      builder.HasKey(x => x.SessionId);

      builder.HasIndex(x => x.IsPersistent);
      builder.HasIndex(x => x.SignedOutById);
      builder.HasIndex(x => x.SignedOutOn);
      builder.HasIndex(x => x.IsActive);

      builder.Property(x => x.Key).HasMaxLength(byte.MaxValue);
      builder.Property(x => x.SignedOutBy).HasColumnType("jsonb");
      builder.Property(x => x.CustomAttributes).HasColumnType("jsonb");

      builder.HasOne(x => x.User).WithMany(x => x.Sessions).OnDelete(DeleteBehavior.Restrict);
    }
  }
}
