using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class ProviderTypeConfiguration : EnumConfiguration<ProviderTypeEntity, int>, IEntityTypeConfiguration<ProviderTypeEntity>
  {
    public override void Configure(EntityTypeBuilder<ProviderTypeEntity> builder)
    {
      base.Configure(builder);

      builder.HasData(ProviderTypeEntity.GetData());
    }
  }
}
