using Logitar.Portal.Domain;
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

    internal DbSet<ApiKeyEntity> ApiKeys { get; private set; } = null!;
    internal DbSet<DictionaryEntity> Dictionaries { get; private set; } = null!;
    internal DbSet<EventEntity> Events { get; private set; } = null!;
    internal DbSet<ExternalProviderEntity> ExternalProviders { get; private set; } = null!;
    internal DbSet<BlacklistedJwtEntity> JwtBlacklist { get; private set; } = null!;
    internal DbSet<LogEventEntity> LogEvents { get; private set; } = null!;
    internal DbSet<LogEntity> Logs { get; private set; } = null!;
    internal DbSet<MessageEntity> Messages { get; private set; } = null!;
    internal DbSet<ProviderTypeEntity> ProviderTypes { get; private set; } = null!;
    internal DbSet<RealmEntity> Realms { get; private set; } = null!;
    internal DbSet<RecipientEntity> Recipients { get; private set; } = null!;
    internal DbSet<RecipientTypeEntity> RecipientTypes { get; private set; } = null!;
    internal DbSet<SenderEntity> Senders { get; private set; } = null!;
    internal DbSet<SessionEntity> Sessions { get; private set; } = null!;
    internal DbSet<TemplateEntity> Templates { get; private set; } = null!;
    internal DbSet<UserEntity> Users { get; private set; } = null!;

    internal async Task<Actor> GetActorAsync(AggregateId id, CancellationToken cancellationToken)
    {
      UserEntity? user = await Users.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == id.Value, cancellationToken);
      if (user != null)
      {
        return new Actor(user);
      }

      ApiKeyEntity? apiKey = await ApiKeys.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AggregateId == id.Value, cancellationToken);
      if (apiKey != null)
      {
        return new Actor(apiKey);
      }

      throw new InvalidOperationException($"The actor 'Id={id}' could not be found.");
    }

    internal async Task UpdateActorsAsync(string id, Actor actor, CancellationToken cancellationToken)
    {
      ApiKeyEntity[] apiKeys = await ApiKeys.Where(x => x.CreatedById == id || x.UpdatedById == id)
        .ToArrayAsync(cancellationToken);
      UpdateAggregateActors(apiKeys, id, actor);

      DictionaryEntity[] dictionaries = await Dictionaries.Where(x => x.CreatedById == id || x.UpdatedById == id)
        .ToArrayAsync(cancellationToken);
      UpdateAggregateActors(dictionaries, id, actor);

      ExternalProviderEntity[] externalProviders = await ExternalProviders.Where(x => x.AddedById == id)
        .ToArrayAsync(cancellationToken);
      foreach (ExternalProviderEntity externalProvider in externalProviders)
      {
        externalProvider.UpdateActors(id, actor);
      }

      MessageEntity[] messages = await Messages.Where(x => x.CreatedById == id || x.UpdatedById == id)
        .ToArrayAsync(cancellationToken);
      UpdateAggregateActors(messages, id, actor);

      RealmEntity[] realms = await Realms.Where(x => x.CreatedById == id || x.UpdatedById == id)
        .ToArrayAsync(cancellationToken);
      UpdateAggregateActors(realms, id, actor);

      SenderEntity[] senders = await Senders.Where(x => x.CreatedById == id || x.UpdatedById == id)
        .ToArrayAsync(cancellationToken);
      UpdateAggregateActors(senders, id, actor);

      SessionEntity[] sessions = await Sessions.Where(x => x.CreatedById == id || x.UpdatedById == id || x.SignedOutById == id)
        .ToArrayAsync(cancellationToken);
      UpdateAggregateActors(sessions, id, actor);

      TemplateEntity[] templates = await Templates.Where(x => x.CreatedById == id || x.UpdatedById == id)
        .ToArrayAsync(cancellationToken);
      UpdateAggregateActors(templates, id, actor);

      UserEntity[] users = await Users.Where(x => x.CreatedById == id || x.UpdatedById == id || x.EmailConfirmedById == id || x.PhoneNumberConfirmedById == id || x.DisabledById == id)
        .ToArrayAsync(cancellationToken);
      UpdateAggregateActors(users, id, actor);
    }
    private static void UpdateAggregateActors(IEnumerable<AggregateEntity> aggregates, string id, Actor actor)
    {
      foreach (AggregateEntity aggregate in aggregates)
      {
        aggregate.UpdateActors(id, actor);
      }
    }

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
