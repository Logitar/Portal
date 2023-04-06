using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Commands;

internal class DeleteRealmHandler : IRequestHandler<DeleteRealm, Realm>
{
  private readonly ICurrentActor _currentActor;
  private readonly IEventStore _eventStore;
  private readonly IRealmQuerier _realmQuerier;

  public DeleteRealmHandler(ICurrentActor currentActor,
    IEventStore eventStore,
    IRealmQuerier realmQuerier)
  {
    _currentActor = currentActor;
    _eventStore = eventStore;
    _realmQuerier = realmQuerier;
  }

  public async Task<Realm> Handle(DeleteRealm request, CancellationToken cancellationToken)
  {
    RealmAggregate realm = await _eventStore.LoadAsync<RealmAggregate>(new AggregateId(request.Id), cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(request.Id);
    Realm output = await _realmQuerier.GetAsync(realm, cancellationToken);

    realm.Delete(_currentActor.Id);

    await _eventStore.SaveAsync(realm, cancellationToken);

    return output;
  }
}
