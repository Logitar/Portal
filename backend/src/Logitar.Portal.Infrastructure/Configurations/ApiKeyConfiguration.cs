using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class ApiKeyConfiguration : AggregateConfiguration<ApiKeyEntity>, IEntityTypeConfiguration<ApiKeyEntity>
  {
    public override void Configure(EntityTypeBuilder<ApiKeyEntity> builder)
    {
      base.Configure(builder);

      builder.HasKey(x => x.ApiKeyId);

      builder.HasIndex(x => x.DisplayName);
      builder.HasIndex(x => x.ExpiresOn);

      builder.Property(x => x.DisplayName).HasMaxLength(256);
    }
  }
}
