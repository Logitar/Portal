using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class BlacklistedJwtConfiguration : IEntityTypeConfiguration<BlacklistedJwtEntity>
  {
    public void Configure(EntityTypeBuilder<BlacklistedJwtEntity> builder)
    {
      builder.HasKey(x => x.BlacklistedJwtId);
      builder.HasIndex(x => x.ExpiresOn);
      builder.HasIndex(x => x.Id).IsUnique();
    }
  }
}
