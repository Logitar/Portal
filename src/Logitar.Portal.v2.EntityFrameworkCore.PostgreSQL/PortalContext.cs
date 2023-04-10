using Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.v2.EntityFrameworkCore.PostgreSQL;

public class PortalContext : DbContext
{
  public PortalContext(DbContextOptions<PortalContext> options) : base(options)
  {
  }

  internal DbSet<ExternalIdentifierEntity> ExternalIdentifiers { get; private set; } = null!;
  internal DbSet<RealmEntity> Realms { get; private set; } = null!;
  internal DbSet<SessionEntity> Sessions { get; private set; } = null!;
  internal DbSet<BlacklistedTokenEntity> TokenBlacklist { get; private set; } = null!;
  internal DbSet<UserEntity> Users { get; private set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortalContext).Assembly);
  }
}
