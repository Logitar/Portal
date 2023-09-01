using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class ApiKeyRoleConfiguration : IEntityTypeConfiguration<ApiKeyRoleEntity>
{
  public void Configure(EntityTypeBuilder<ApiKeyRoleEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.ApiKeyRoles));
    builder.HasKey(x => new { x.ApiKeyId, x.RoleId });
  }
}
