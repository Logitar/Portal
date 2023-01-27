using Logitar.Portal.Infrastructure2.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure2
{
  public class PortalContext : DbContext
  {
    public PortalContext(DbContextOptions<PortalContext> options) : base(options)
    {
    }

    internal DbSet<ActorEntity> Actors { get; private set; } = null!;
    internal DbSet<ActorTypeEntity> ActorTypes { get; private set; } = null!;
    internal DbSet<EventEntity> Events { get; private set; } = null!;
    internal DbSet<RealmEntity> Realms { get; private set; } = null!;
    internal DbSet<SessionEntity> Sessions { get; private set; } = null!;
    internal DbSet<UserEntity> Users { get; private set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortalContext).Assembly);
    }
  }
}
