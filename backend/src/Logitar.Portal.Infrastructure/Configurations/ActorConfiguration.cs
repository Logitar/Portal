using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class ActorConfiguration : IEntityTypeConfiguration<ActorEntity>
  {
    public void Configure(EntityTypeBuilder<ActorEntity> builder)
    {
      builder.HasKey(x => x.ActorId);

      builder.HasIndex(x => x.AggregateId).IsUnique();

      builder.Property(x => x.AggregateId).HasMaxLength(256);
      builder.Property(x => x.DisplayName).HasMaxLength(512);
      builder.Property(x => x.Email).HasMaxLength(256);
      builder.Property(x => x.Picture).HasMaxLength(2048);
    }
  }
}
