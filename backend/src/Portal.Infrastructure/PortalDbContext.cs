using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Portal.Core;
using Portal.Core.ApiKeys;
using Portal.Core.Realms;
using Portal.Core.Sessions;
using Portal.Core.Users;
using Portal.Infrastructure.Entities;

namespace Portal.Infrastructure
{
  public class PortalDbContext : DbContext
  {
    private readonly IConfiguration _configuration;

    public PortalDbContext(IConfiguration configuration, DbContextOptions<PortalDbContext> options)
      : base(options)
    {
      _configuration = configuration;
    }

    internal DbSet<ApiKey> ApiKeys { get; private set; } = null!;
    internal DbSet<EventEntity> Events { get; private set; } = null!;
    internal DbSet<BlacklistedJwt> JwtBlacklist { get; private set; } = null!;
    internal DbSet<Realm> Realms { get; private set; } = null!;
    internal DbSet<Session> Sessions { get; private set; } = null!;
    internal DbSet<User> Users { get; private set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder builder)
    {
      builder.UseNpgsql(_configuration.GetValue<string>($"POSTGRESQLCONNSTR_{nameof(PortalDbContext)}"));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
      builder.ApplyConfigurationsFromAssembly(typeof(PortalDbContext).Assembly);

      builder.Ignore<EventBase>();

      builder.HasPostgresExtension("uuid-ossp");
    }
  }
}
