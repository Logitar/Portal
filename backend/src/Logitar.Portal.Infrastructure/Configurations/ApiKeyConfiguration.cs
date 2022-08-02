using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Logitar.Portal.Core.ApiKeys;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class ApiKeyConfiguration : AggregateConfiguration<ApiKey>, IEntityTypeConfiguration<ApiKey>
  {
    public override void Configure(EntityTypeBuilder<ApiKey> builder)
    {
      base.Configure(builder);

      builder.HasIndex(x => x.IsExpired);
      builder.HasIndex(x => x.Name);

      builder.Property(x => x.IsExpired).HasDefaultValue(false);
      builder.Property(x => x.Name).HasMaxLength(256);
    }
  }
}
