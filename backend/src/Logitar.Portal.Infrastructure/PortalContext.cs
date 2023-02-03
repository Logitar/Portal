using Logitar.Portal.Infrastructure.Entities;
using Logitar.Portal.Infrastructure.ValueConverters;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Logitar.Portal.Infrastructure
{
  public class PortalContext : DbContext
  {
    public PortalContext(DbContextOptions<PortalContext> options) : base(options)
    {
    }

    internal DbSet<ActorEntity> Actors { get; private set; } = null!;
    internal DbSet<ActorTypeEntity> ActorTypes { get; private set; } = null!;
    internal DbSet<ApiKeyEntity> ApiKeys { get; private set; } = null!;
    internal DbSet<DictionaryEntity> Dictionaries { get; private set; } = null!;
    internal DbSet<EventEntity> Events { get; private set; } = null!;
    internal DbSet<ExternalProviderEntity> ExternalProviders { get; private set; } = null!;
    internal DbSet<BlacklistedJwtEntity> JwtBlacklist { get; private set; } = null!;
    internal DbSet<RealmEntity> Realms { get; private set; } = null!;
    internal DbSet<ProviderTypeEntity> SenderProviderTypes { get; private set; } = null!;
    internal DbSet<SenderEntity> Senders { get; private set; } = null!;
    internal DbSet<SessionEntity> Sessions { get; private set; } = null!;
    internal DbSet<TemplateEntity> Templates { get; private set; } = null!;
    internal DbSet<UserEntity> Users { get; private set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
      builder.ApplyConfigurationsFromAssembly(typeof(PortalContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder builder)
    {
      builder.Properties<CultureInfo>().HaveConversion<CultureInfoValueConverter>();
    }
  }
}
