using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Logitar.Portal.Infrastructure.Entities;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class BlacklistedJwtConfiguration : IEntityTypeConfiguration<BlacklistedJwt>
  {
    public virtual void Configure(EntityTypeBuilder<BlacklistedJwt> builder)
    {
      builder.HasKey(x => x.Sid);
      builder.HasIndex(x => x.ExpiresAt);
      builder.HasIndex(x => x.Id).IsUnique();
    }
  }
}
