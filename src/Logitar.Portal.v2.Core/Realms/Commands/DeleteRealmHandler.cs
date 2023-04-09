using Logitar.Portal.v2.Contracts.Realms;
using Logitar.Portal.v2.Core.Users.Commands;
using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Commands;

internal class DeleteRealmHandler : IRequestHandler<DeleteRealm, Realm>
{
  private readonly ICurrentActor _currentActor;
  private readonly IMediator _mediator;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public DeleteRealmHandler(ICurrentActor currentActor,
    IMediator mediator,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository)
  {
    _currentActor = currentActor;
    _mediator = mediator;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm> Handle(DeleteRealm request, CancellationToken cancellationToken)
  {
    RealmAggregate realm = await _realmRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(request.Id);
    Realm output = await _realmQuerier.GetAsync(realm, cancellationToken);

    await _mediator.Send(new DeleteUsers(realm), cancellationToken);

    realm.Delete(_currentActor.Id);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return output;
  }
}
