using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class ApiKeyConfiguration : AggregateConfiguration<ApiKeyEntity>, IEntityTypeConfiguration<ApiKeyEntity>
{
  public override void Configure(EntityTypeBuilder<ApiKeyEntity> builder)
  {
    base.Configure(builder);

    builder.ToTable(nameof(PortalContext.ApiKeys));
    builder.HasKey(x => x.ApiKeyId);

    builder.HasIndex(x => x.TenantId);
    builder.HasIndex(x => x.DisplayName);
    builder.HasIndex(x => x.ExpiresOn);
    builder.HasIndex(x => x.AuthenticatedOn);

    builder.Ignore(x => x.CustomAttributes);

    builder.Property(x => x.TenantId).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.Secret).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.DisplayName).HasMaxLength(byte.MaxValue);
    builder.Property(x => x.CustomAttributesSerialized).HasColumnName(nameof(ApiKeyEntity.CustomAttributes));

    builder.HasMany(x => x.Roles).WithMany(x => x.ApiKeys).UsingEntity<ApiKeyRoleEntity>();
  }
}
