using Logitar.Portal.Core.Actors;
using Logitar.Portal.Core.ApiKeys;
using Logitar.Portal.Core.Users;
using Microsoft.EntityFrameworkCore;

namespace Logitar.Portal.Infrastructure.Actors
{
  internal class ActorService : IActorService
  {
    private readonly PortalDbContext _dbContext;

    public ActorService(PortalDbContext dbContext)
    {
      _dbContext = dbContext;
    }

    public async Task SaveAsync(ApiKey apiKey, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(apiKey);

      Actor? actor = await _dbContext.Actors
        .SingleOrDefaultAsync(x => x.Type == ActorType.ApiKey && x.Id == apiKey.Id, cancellationToken);

      if (actor == null)
      {
        actor = new Actor(apiKey);
        _dbContext.Actors.Add(actor);
      }
      else
      {
        actor.Update(apiKey);
        _dbContext.Actors.Update(actor);
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(User user, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(user);

      Actor? actor = await _dbContext.Actors
        .SingleOrDefaultAsync(x => x.Type == ActorType.User && x.Id == user.Id, cancellationToken);

      if (actor == null)
      {
        actor = new Actor(user);
        _dbContext.Actors.Add(actor);
      }
      else
      {
        actor.Update(user);
        _dbContext.Actors.Update(actor);
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(IEnumerable<ApiKey> apiKeys, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(apiKeys);

      HashSet<Guid> ids = apiKeys.Select(x => x.Id).ToHashSet();
      Dictionary<Guid, Actor> actors = await _dbContext.Actors
        .Where(x => x.Type == ActorType.ApiKey && ids.Contains(x.Id))
        .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

      foreach (ApiKey apiKey in apiKeys)
      {
        if (actors.TryGetValue(apiKey.Id, out Actor? actor))
        {
          actor.Update(apiKey);
          _dbContext.Actors.Update(actor);
        }
        else
        {
          actor = new Actor(apiKey);
          _dbContext.Actors.Add(actor);
        }
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveAsync(IEnumerable<User> users, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(users);

      HashSet<Guid> ids = users.Select(x => x.Id).ToHashSet();
      Dictionary<Guid, Actor> actors = await _dbContext.Actors
        .Where(x => x.Type == ActorType.User && ids.Contains(x.Id))
        .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

      foreach (User user in users)
      {
        if (actors.TryGetValue(user.Id, out Actor? actor))
        {
          actor.Update(user);
          _dbContext.Actors.Update(actor);
        }
        else
        {
          actor = new Actor(user);
          _dbContext.Actors.Add(actor);
        }
      }

      await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SynchronizeAsync(CancellationToken cancellationToken)
    {
      ApiKey[] apiKeys = await _dbContext.ApiKeys.AsNoTracking().ToArrayAsync(cancellationToken);
      await SaveAsync(apiKeys, cancellationToken);

      User[] users = await _dbContext.Users.AsNoTracking().ToArrayAsync(cancellationToken);
      await SaveAsync(users, cancellationToken);
    }
  }
}
