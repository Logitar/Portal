using Logitar.Portal.Application;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Templates;
using Logitar.Portal.Domain.Users;
using Logitar.Portal.Infrastructure.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Logitar.Portal.Infrastructure
{
  internal class Repository : IRepository
  {
    private readonly PortalContext _context;
    private readonly IInternalLoggingContext _log;
    private readonly IPublisher _publisher;

    public Repository(PortalContext context, IInternalLoggingContext log, IPublisher publisher)
    {
      _context = context;
      _log = log;
      _publisher = publisher;
    }

    public async Task<T?> LoadAsync<T>(string id, CancellationToken cancellationToken) where T : AggregateRoot
      => await LoadAsync<T>(new AggregateId(id), cancellationToken);
    public async Task<T?> LoadAsync<T>(AggregateId id, CancellationToken cancellationToken) where T : AggregateRoot
    {
      string aggregateType = typeof(T).GetName();

      EventEntity[] events = await _context.Events.AsNoTracking()
        .Where(x => x.AggregateType == aggregateType && x.AggregateId == id.Value)
        .OrderBy(x => x.Version)
        .ToArrayAsync(cancellationToken);

      if (!events.Any())
      {
        return null;
      }

      T aggregate = AggregateRoot.LoadFromHistory<T>(events.Select(e => e.Deserialize()), id);

      return aggregate.IsDeleted ? null : aggregate;
    }

    public async Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<AggregateId> ids, CancellationToken cancellationToken) where T : AggregateRoot
      => await LoadAsync<T>(ids.Select(id => id.Value), cancellationToken);
    public async Task<IEnumerable<T>> LoadAsync<T>(IEnumerable<string> ids, CancellationToken cancellationToken) where T : AggregateRoot
    {
      string aggregateType = typeof(T).GetName();

      EventEntity[] events = await _context.Events.AsNoTracking()
        .Where(x => x.AggregateType == aggregateType && ids.Contains(x.AggregateId))
        .OrderBy(x => x.Version)
        .ToArrayAsync(cancellationToken);

      if (!events.Any())
      {
        return Enumerable.Empty<T>();
      }

      return events.GroupBy(x => x.AggregateId)
        .Select(x => AggregateRoot.LoadFromHistory<T>(x.Select(e => e.Deserialize()), new AggregateId(x.Key)))
        .Where(x => !x.IsDeleted)
        .ToArray();
    }

    public async Task<Configuration?> LoadConfigurationAsync(CancellationToken cancellationToken)
    {
      return await LoadAsync<Configuration>(Configuration.AggregateId, cancellationToken);
    }
    public async Task<IEnumerable<Dictionary>> LoadDictionariesByRealmAsync(Realm? realm, CancellationToken cancellationToken)
    {
      DictionaryEntity[] dictionaries = await _context.Dictionaries.AsNoTracking()
        .Include(x => x.Realm)
        .Where(x => (realm == null ? x.RealmId == null : x.Realm!.AggregateId == realm.Id.Value))
        .ToArrayAsync(cancellationToken);

      return dictionaries.Any() ? await LoadAsync<Dictionary>(dictionaries.Select(x => x.AggregateId), cancellationToken) : Enumerable.Empty<Dictionary>();
    }

    public async Task<Realm?> LoadRealmByAliasOrIdAsync(string aliasOrId, CancellationToken cancellationToken)
    {
      RealmEntity? realm = await _context.Realms.AsNoTracking()
        .SingleOrDefaultAsync(x => x.AliasNormalized == aliasOrId.ToUpper()
          || x.AggregateId == aliasOrId, cancellationToken);

      return realm == null ? null : await LoadAsync<Realm>(realm.AggregateId, cancellationToken);
    }

    public async Task<Sender?> LoadDefaultSenderAsync(Realm? realm, CancellationToken cancellationToken)
      => await LoadDefaultSenderAsync(realm?.Id, cancellationToken);
    public async Task<Sender?> LoadDefaultSenderAsync(AggregateId? realmId, CancellationToken cancellationToken)
    {
      SenderEntity? sender = await _context.Senders.AsNoTracking()
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => (realmId.HasValue ? x.Realm!.AggregateId == realmId.Value.Value : x.RealmId == null)
          && x.IsDefault, cancellationToken);

      return sender == null ? null : await LoadAsync<Sender>(sender.AggregateId, cancellationToken);
    }
    public async Task<IEnumerable<Sender>> LoadSendersByRealmAsync(Realm realm, CancellationToken cancellationToken)
    {
      SenderEntity[] senders = await _context.Senders.AsNoTracking()
        .Include(x => x.Realm)
        .Where(x => x.Realm!.AggregateId == realm.Id.Value)
        .ToArrayAsync(cancellationToken);

      return senders.Any() ? await LoadAsync<Sender>(senders.Select(x => x.AggregateId), cancellationToken) : Enumerable.Empty<Sender>();
    }

    public async Task<IEnumerable<Session>> LoadActiveSessionsByUserAsync(User user, CancellationToken cancellationToken)
    {
      SessionEntity[] sessions = await _context.Sessions.AsNoTracking()
        .Include(x => x.User)
        .Where(x => x.User!.AggregateId == user.Id.Value && x.IsActive)
        .ToArrayAsync(cancellationToken);

      if (!sessions.Any())
      {
        return Enumerable.Empty<Session>();
      }

      return await LoadAsync<Session>(sessions.Select(x => x.AggregateId), cancellationToken);
    }
    public async Task<IEnumerable<Session>> LoadSessionsByRealmAsync(Realm realm, CancellationToken cancellationToken)
    {
      SessionEntity[] sessions = await _context.Sessions.AsNoTracking()
        .Include(x => x.User).ThenInclude(x => x!.Realm)
        .Where(x => x.User!.Realm!.AggregateId == realm.Id.Value)
        .ToArrayAsync(cancellationToken);

      return sessions.Any() ? await LoadAsync<Session>(sessions.Select(x => x.AggregateId), cancellationToken) : Enumerable.Empty<Session>();
    }
    public async Task<IEnumerable<Session>> LoadSessionsByUserAsync(User user, CancellationToken cancellationToken)
    {
      SessionEntity[] sessions = await _context.Sessions.AsNoTracking()
        .Include(x => x.User)
        .Where(x => x.User!.AggregateId == user.Id.Value)
        .ToArrayAsync(cancellationToken);

      return sessions.Any() ? await LoadAsync<Session>(sessions.Select(x => x.AggregateId), cancellationToken) : Enumerable.Empty<Session>();
    }

    public async Task<Template?> LoadTemplateByIdOrKeyAsync(string idOrKey, Realm? realm, CancellationToken cancellationToken)
    {
      TemplateEntity? template = await _context.Templates.AsNoTracking()
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => (realm == null ? x.RealmId == null : x.Realm!.AggregateId == realm.Id.Value)
          && (x.AggregateId == idOrKey || x.KeyNormalized == idOrKey.ToUpper()), cancellationToken);

      return template == null ? null : await LoadAsync<Template>(template.AggregateId, cancellationToken);
    }
    public async Task<Template?> LoadTemplateByKeyAsync(string key, Realm? realm, CancellationToken cancellationToken)
    {
      TemplateEntity? template = await _context.Templates.AsNoTracking()
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => (realm == null ? x.RealmId == null : x.Realm!.AggregateId == realm.Id.Value)
          && x.KeyNormalized == key.ToUpper(), cancellationToken);

      return template == null ? null : await LoadAsync<Template>(template.AggregateId, cancellationToken);
    }
    public async Task<IEnumerable<Template>> LoadTemplatesByRealmAsync(Realm realm, CancellationToken cancellationToken)
    {
      TemplateEntity[] templates = await _context.Templates.AsNoTracking()
        .Include(x => x.Realm)
        .Where(x => x.Realm!.AggregateId == realm.Id.Value)
        .ToArrayAsync(cancellationToken);

      return templates.Any() ? await LoadAsync<Template>(templates.Select(x => x.AggregateId), cancellationToken) : Enumerable.Empty<Template>();
    }

    public async Task<User?> LoadUserByExternalProviderAsync(Realm realm, string key, string value, CancellationToken cancellationToken)
    {
      UserEntity? user = await _context.Users.AsNoTracking()
        .Include(x => x.ExternalProviders)
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => x.Realm!.AggregateId == realm.Id.Value
          && x.ExternalProviders.Any(y => y.Key == key && y.Value == value), cancellationToken);

      return user == null ? null : await LoadAsync<User>(user.AggregateId, cancellationToken);
    }
    public async Task<User?> LoadUserByUsernameAsync(string username, Realm? realm, CancellationToken cancellationToken)
    {
      UserEntity? user = await _context.Users.AsNoTracking()
        .Include(x => x.Realm)
        .SingleOrDefaultAsync(x => (realm == null ? x.RealmId == null : x.Realm!.AggregateId == realm.Id.Value)
          && x.UsernameNormalized == username.ToUpper(), cancellationToken);

      return user == null ? null : await LoadAsync<User>(user.AggregateId, cancellationToken);
    }
    public async Task<IEnumerable<User>> LoadUsersByEmailAsync(string email, Realm? realm, CancellationToken cancellationToken)
    {
      UserEntity[] users = await _context.Users.AsNoTracking()
        .Include(x => x.Realm)
        .Where(x => (realm == null ? x.RealmId == null : x.Realm!.AggregateId == realm.Id.Value)
          && x.EmailNormalized == email.ToUpper())
        .ToArrayAsync(cancellationToken);

      return users.Any() ? await LoadAsync<User>(users.Select(x => x.AggregateId), cancellationToken) : Enumerable.Empty<User>();
    }
    public async Task<IEnumerable<User>> LoadUsersByRealmAsync(Realm realm, CancellationToken cancellationToken)
    {
      UserEntity[] users = await _context.Users.AsNoTracking()
        .Include(x => x.Realm)
        .Where(x => x.Realm!.AggregateId == realm.Id.Value)
        .ToArrayAsync(cancellationToken);

      return users.Any() ? await LoadAsync<User>(users.Select(x => x.AggregateId), cancellationToken) : Enumerable.Empty<User>();
    }
    public async Task<IEnumerable<User>> LoadUsersByUsernamesAsync(IEnumerable<string> usernames, Realm? realm, CancellationToken cancellationToken)
    {
      usernames = usernames.Select(x => x.ToUpper());

      UserEntity[] users = await _context.Users.AsNoTracking()
        .Include(x => x.Realm)
        .Where(x => (realm == null ? x.RealmId == null : x.Realm!.AggregateId == realm.Id.Value) && usernames.Contains(x.UsernameNormalized))
        .ToArrayAsync(cancellationToken);

      return users.Any() ? await LoadAsync<User>(users.Select(x => x.AggregateId), cancellationToken) : Enumerable.Empty<User>();
    }

    public async Task SaveAsync(AggregateRoot aggregate, CancellationToken cancellationToken)
    {
      if (aggregate.HasChanges)
      {
        IEnumerable<EventEntity> events = EventEntity.FromChanges(aggregate);
        _context.Events.AddRange(events);
        await _context.SaveChangesAsync(cancellationToken);

        _log.AddEvents(events);

        foreach (DomainEvent change in aggregate.Changes)
        {
          await _publisher.Publish(change, cancellationToken);
        }

        aggregate.ClearChanges();
      }
    }

    public async Task SaveAsync(IEnumerable<AggregateRoot> aggregates, CancellationToken cancellationToken)
    {
      IEnumerable<EventEntity> events = aggregates.SelectMany(EventEntity.FromChanges);
      if (events.Any())
      {
        _context.Events.AddRange(events);
        await _context.SaveChangesAsync(cancellationToken);

        _log.AddEvents(events);

        foreach (AggregateRoot aggregate in aggregates)
        {
          if (aggregate.HasChanges)
          {
            foreach (DomainEvent change in aggregate.Changes)
            {
              await _publisher.Publish(change, cancellationToken);
            }

            aggregate.ClearChanges();
          }
        }
      }
    }
  }
}
