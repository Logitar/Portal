using Logitar.Portal.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.PostgreSQL.Configurations;

internal class ExternalIdentifierConfiguration : IEntityTypeConfiguration<ExternalIdentifierEntity>
{
  public void Configure(EntityTypeBuilder<ExternalIdentifierEntity> builder)
  {
    builder.HasKey(x => x.ExternalIdentifierId);

    builder.HasIndex(x => x.Id).IsUnique();
    builder.HasIndex(x => x.CreatedById);
    builder.HasIndex(x => x.UpdatedById);
    builder.HasIndex(x => new { x.RealmId, x.Key, x.ValueNormalized }).IsUnique();

    builder.Property(x => x.Key).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Value).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.ValueNormalized).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.CreatedBy).HasColumnType("jsonb");
    builder.Property(x => x.UpdatedBy).HasColumnType("jsonb");

    builder.HasOne(x => x.Realm).WithMany(x => x.ExternalIdentifiers).OnDelete(DeleteBehavior.Cascade);
    builder.HasOne(x => x.User).WithMany(x => x.ExternalIdentifiers).OnDelete(DeleteBehavior.Cascade);
  }
}
