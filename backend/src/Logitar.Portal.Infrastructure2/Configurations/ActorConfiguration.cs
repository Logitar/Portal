using Logitar.Portal.Infrastructure2.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure2.Configurations
{
  internal class ActorConfiguration : IEntityTypeConfiguration<ActorEntity>
  {
    public void Configure(EntityTypeBuilder<ActorEntity> builder)
    {
      builder.HasKey(x => x.ActorId);

      builder.HasIndex(x => x.AggregateId).IsUnique();

      builder.Property(x => x.AggregateId).HasMaxLength(256);
      builder.Property(x => x.DisplayName).HasMaxLength(386);
      builder.Property(x => x.Email).HasMaxLength(256);
      builder.Property(x => x.Email).HasMaxLength(2048);
    }
  }
}
