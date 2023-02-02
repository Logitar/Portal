using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class ExternalProviderConfiguration : IEntityTypeConfiguration<ExternalProviderEntity>
  {
    public void Configure(EntityTypeBuilder<ExternalProviderEntity> builder)
    {
      builder.HasKey(x => x.ExternalProviderId);

      builder.HasIndex(x => x.Id).IsUnique();
      builder.HasIndex(x => new { x.RealmId, x.Key, x.Value }).IsUnique();

      builder.Property(x => x.Key).HasMaxLength(256);
      builder.Property(x => x.Value).HasMaxLength(256);
      builder.Property(x => x.DisplayName).HasMaxLength(256);
      builder.Property(x => x.AddedBy).HasMaxLength(256);
    }
  }
}
