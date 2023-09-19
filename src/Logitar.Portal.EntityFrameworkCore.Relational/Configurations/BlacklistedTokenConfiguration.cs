using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class BlacklistedTokenConfiguration : IEntityTypeConfiguration<BlacklistedTokenEntity>
{
  public void Configure(EntityTypeBuilder<BlacklistedTokenEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.TokenBlacklist));
    builder.HasKey(x => x.BlacklistedTokenId);

    builder.HasIndex(x => x.ExpiresOn);
    builder.HasIndex(x => x.Id).IsUnique();
  }
}
