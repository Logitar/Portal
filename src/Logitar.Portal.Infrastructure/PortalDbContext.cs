using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Actors;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Emails.Messages;
using Logitar.Portal.Domain.Emails.Senders;
using Logitar.Portal.Domain.Emails.Templates;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Logitar.Portal.Infrastructure
{
  public class PortalDbContext : DbContext
  {
    private readonly IConfiguration _configuration;

    public PortalDbContext(IConfiguration configuration, DbContextOptions<PortalDbContext> options)
      : base(options)
    {
      _configuration = configuration;
    }

    public DbSet<Actor> Actors { get; private set; } = null!;
    public DbSet<ApiKey> ApiKeys { get; private set; } = null!;
    public DbSet<Dictionary> Dictionaries { get; private set; } = null!;
    public DbSet<Event> Events { get; private set; } = null!;
    public DbSet<BlacklistedJwt> JwtBlacklist { get; private set; } = null!;
    public DbSet<Log> Logs { get; private set; } = null!;
    public DbSet<Message> Messages { get; private set; } = null!;
    public DbSet<Realm> Realms { get; private set; } = null!;
    public DbSet<Sender> Senders { get; private set; } = null!;
    public DbSet<Session> Sessions { get; private set; } = null!;
    public DbSet<Template> Templates { get; private set; } = null!;
    public DbSet<User> Users { get; private set; } = null!;

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
