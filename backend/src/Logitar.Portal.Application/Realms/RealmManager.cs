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

  public async Task SaveAsync(Realm realm, ActorId actorId, CancellationToken cancellationToken)
  {
    bool hasBeenDeleted = false;
    bool hasUniqueNameChanged = false;
    foreach (IEvent change in realm.Changes)
    {
      if (change is RealmCreated || change is RealmUniqueSlugChanged)
      {
        hasUniqueNameChanged = true;
      }
      else if (change is RealmDeleted)
      {
        hasBeenDeleted = true;
      }
    }

    if (hasUniqueNameChanged)
    {
      Realm? conflict = await _realmRepository.LoadAsync(realm.UniqueSlug, cancellationToken);
      if (conflict != null && !conflict.Equals(realm))
      {
        throw new UniqueSlugAlreadyUsedException(realm, conflict.Id);
      }
    }

    if (hasBeenDeleted)
    {
      await _publisher.Publish(new DeleteRealmSessionsCommand(realm, actorId), cancellationToken);
      await _publisher.Publish(new DeleteRealmUsersCommand(realm, actorId), cancellationToken);
      await _publisher.Publish(new DeleteRealmApiKeysCommand(realm, actorId), cancellationToken);
      await _publisher.Publish(new DeleteRealmRolesCommand(realm, actorId), cancellationToken);
      await _publisher.Publish(new DeleteRealmOneTimePasswordsCommand(realm, actorId), cancellationToken);

      await _publisher.Publish(new DeleteRealmMessagesCommand(realm, actorId), cancellationToken);
      await _publisher.Publish(new DeleteRealmDictionariesCommand(realm, actorId), cancellationToken);
      await _publisher.Publish(new DeleteRealmSendersCommand(realm, actorId), cancellationToken);
      await _publisher.Publish(new DeleteRealmTemplatesCommand(realm, actorId), cancellationToken);
    }

    await _realmRepository.SaveAsync(realm, cancellationToken);
  }
}
