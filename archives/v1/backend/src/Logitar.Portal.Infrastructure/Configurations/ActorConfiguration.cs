using Logitar.Portal.Domain.Actors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class ActorConfiguration : IEntityTypeConfiguration<Actor>
  {
    public void Configure(EntityTypeBuilder<Actor> builder)
    {
      builder.HasKey(x => x.Sid);
      builder.HasIndex(x => x.Id).IsUnique();
      builder.HasIndex(x => x.Type);

      builder.Property(x => x.Email).HasMaxLength(256);
      builder.Property(x => x.Name).HasMaxLength(256);
      builder.Property(x => x.Picture).HasMaxLength(2048);
    }
  }
}
