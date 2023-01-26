using Logitar.Portal.Infrastructure2.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure2.Configurations
{
  internal class ConfigurationConfiguration : IEntityTypeConfiguration<ConfigurationEntity>
  {
    public void Configure(EntityTypeBuilder<ConfigurationEntity> builder)
    {
      builder.HasKey(x => x.Key);

      builder.Property(x => x.Key).HasMaxLength(256);
    }
  }
}
