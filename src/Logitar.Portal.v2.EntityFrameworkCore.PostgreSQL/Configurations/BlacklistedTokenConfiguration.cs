using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Configurations;

internal class BlacklistedTokenConfiguration : IEntityTypeConfiguration<BlacklistedTokenEntity>
{
  public void Configure(EntityTypeBuilder<BlacklistedTokenEntity> builder)
  {
    builder.HasKey(x => x.BlacklistedTokenId);

    builder.HasIndex(x => x.ExpiresOn);
    builder.HasIndex(x => x.Id).IsUnique();
  }
}
