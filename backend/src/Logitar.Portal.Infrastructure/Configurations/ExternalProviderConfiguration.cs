using Logitar.Portal.Core.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.Infrastructure.Configurations
{
  internal class ExternalProviderConfiguration : IEntityTypeConfiguration<ExternalProvider>
  {
    public void Configure(EntityTypeBuilder<ExternalProvider> builder)
    {
      builder.ToTable("ExternalProviders");

      builder.HasKey(x => x.Sid);
      builder.HasIndex(x => x.Id).IsUnique();
      builder.HasIndex(x => new { x.Key, x.Value });

      builder.HasOne(x => x.User).WithMany(x => x.ExternalProviders).OnDelete(DeleteBehavior.Cascade);

      builder.Property(x => x.AddedAt).HasDefaultValueSql("now()");
      builder.Property(x => x.AddedById).HasDefaultValue(Guid.Empty);
      builder.Property(x => x.DisplayName).HasMaxLength(256);
      builder.Property(x => x.Id).HasDefaultValueSql("uuid_generate_v4()");
      builder.Property(x => x.Key).HasMaxLength(256);
      builder.Property(x => x.Value).HasMaxLength(256);
    }
  }
}
