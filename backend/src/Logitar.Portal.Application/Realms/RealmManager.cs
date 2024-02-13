using Logitar.EventSourcing;
using Logitar.Portal.Application.Realms.DeleteCommands;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Realms.Events;
using MediatR;

namespace Logitar.Portal.Application.Realms;

internal class RealmManager : IRealmManager
{
  private readonly IPublisher _publisher;
  private readonly IRealmRepository _realmRepository;

  public RealmManager(IPublisher publisher, IRealmRepository realmRepository)
  {
    _publisher = publisher;
    _realmRepository = realmRepository;
  }

  public async Task SaveAsync(RealmAggregate realm, ActorId actorId, CancellationToken cancellationToken)
  {
    bool hasBeenDeleted = false;
    bool hasUniqueNameChanged = false;
    foreach (DomainEvent change in realm.Changes)
    {
      if (change is RealmCreatedEvent || change is RealmUniqueSlugChangedEvent)
      {
        hasUniqueNameChanged = true;
      }
      else if (change is RealmDeletedEvent)
      {
        hasBeenDeleted = true;
      }
    }

    if (hasUniqueNameChanged)
    {
      RealmAggregate? other = await _realmRepository.LoadAsync(realm.UniqueSlug, cancellationToken);
      if (other?.Equals(realm) == false)
      {
        throw new UniqueSlugAlreadyUsedException(realm.UniqueSlug);
      }
    }

    if (hasBeenDeleted)
    {
      await _publisher.Publish(new DeleteRealmSessionsCommand(realm, actorId), cancellationToken);
      await _publisher.Publish(new DeleteRealmUsersCommand(realm, actorId), cancellationToken);
      await _publisher.Publish(new DeleteRealmApiKeysCommand(realm, actorId), cancellationToken);
      await _publisher.Publish(new DeleteRealmRolesCommand(realm, actorId), cancellationToken);
      await _publisher.Publish(new DeleteRealmOneTimePasswordsCommand(realm, actorId), cancellationToken);

      await _publisher.Publish(new DeleteRealmDictionariesCommand(realm, actorId), cancellationToken);
    }

    await _realmRepository.SaveAsync(realm, cancellationToken);
  }
}
