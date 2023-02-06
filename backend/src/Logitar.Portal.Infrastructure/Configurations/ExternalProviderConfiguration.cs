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

      builder.HasOne(x => x.Realm).WithMany(x => x.ExternalProviders).OnDelete(DeleteBehavior.Cascade);
      builder.HasOne(x => x.User).WithMany(x => x.ExternalProviders).OnDelete(DeleteBehavior.Cascade);

      builder.HasIndex(x => x.Id).IsUnique();
      builder.HasIndex(x => x.AddedById);
      builder.HasIndex(x => new { x.RealmId, x.Key, x.Value }).IsUnique();

      builder.Property(x => x.Key).HasMaxLength(255);
      builder.Property(x => x.Value).HasMaxLength(255);
      builder.Property(x => x.DisplayName).HasMaxLength(255);
      builder.Property(x => x.AddedById).HasMaxLength(255);
      builder.Property(x => x.AddedBy).HasColumnType("jsonb");
    }
  }
}
