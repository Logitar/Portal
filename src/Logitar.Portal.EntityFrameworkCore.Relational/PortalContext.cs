using Logitar.Portal.EntityFrameworkCore.Relational.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.EntityFrameworkCore.Relational;

public class PortalContext : DbContext
{
  public PortalContext(DbContextOptions<PortalContext> options) : base(options)
  {
  }

  internal DbSet<ActorEntity> Actors { get; private set; }
  internal DbSet<ApiKeyEntity> ApiKeys { get; private set; }
  internal DbSet<ApiKeyRoleEntity> ApiKeyRoles { get; private set; }
  internal DbSet<LogEntity> Logs { get; private set; }
  internal DbSet<LogEventEntity> LogEvents { get; private set; }
  internal DbSet<RealmEntity> Realms { get; private set; }
  internal DbSet<RoleEntity> Roles { get; private set; }
  internal DbSet<SessionEntity> Sessions { get; private set; }
  internal DbSet<BlacklistedTokenEntity> TokenBlacklist { get; private set; }
  internal DbSet<UserEntity> Users { get; private set; }
  internal DbSet<UserIdentifierEntity> UserIdentifiers { get; private set; }
  internal DbSet<UserRoleEntity> UserRoles { get; private set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortalContext).Assembly);
  }
}
