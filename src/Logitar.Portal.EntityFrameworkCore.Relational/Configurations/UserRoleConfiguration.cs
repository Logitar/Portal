using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Logitar.Portal.EntityFrameworkCore.Relational.Configurations;

internal class UserRoleConfiguration : IEntityTypeConfiguration<UserRoleEntity>
{
  public void Configure(EntityTypeBuilder<UserRoleEntity> builder)
  {
    builder.ToTable(nameof(PortalContext.UserRoles));
    builder.HasKey(x => new { x.UserId, x.RoleId });
  }
}
