using Logitar.Portal.Infrastructure2.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure2.Configurations
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
