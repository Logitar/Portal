using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class ActorTypeConfiguration : EnumConfiguration<ActorTypeEntity, int>, IEntityTypeConfiguration<ActorTypeEntity>
  {
    public override void Configure(EntityTypeBuilder<ActorTypeEntity> builder)
    {
      base.Configure(builder);

      builder.HasData(ActorTypeEntity.GetData());
    }
  }
}
