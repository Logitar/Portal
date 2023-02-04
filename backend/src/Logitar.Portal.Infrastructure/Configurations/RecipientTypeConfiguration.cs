using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class RecipientTypeConfiguration : EnumConfiguration<RecipientTypeEntity, int>, IEntityTypeConfiguration<RecipientTypeEntity>
  {
    public override void Configure(EntityTypeBuilder<RecipientTypeEntity> builder)
    {
      base.Configure(builder);

      builder.HasData(RecipientTypeEntity.GetData());
    }
  }
}
