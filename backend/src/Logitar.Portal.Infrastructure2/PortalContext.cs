using Logitar.Portal.Infrastructure2.Entities;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure2
{
  internal class PortalContext : DbContext
  {
    public PortalContext(DbContextOptions<PortalContext> options) : base(options)
    {
    }

    public DbSet<ActorEntity> Actors { get; private set; } = null!;
    public DbSet<ActorTypeEntity> ActorTypes { get; private set; } = null!;
    public DbSet<ConfigurationEntity> Configurations { get; private set; } = null!;
    public DbSet<EventEntity> Events { get; private set; } = null!;
    public DbSet<UserEntity> Users { get; private set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(typeof(PortalContext).Assembly);
    }
  }
}
