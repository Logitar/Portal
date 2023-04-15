using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Core.Dictionaries.Commands;
using Logitar.Portal.Core.Senders.Commands;
using Logitar.Portal.Core.Sessions.Commands;
using Logitar.Portal.Core.Templates.Commands;
using Logitar.Portal.Core.Users.Commands;
using MediatR;

namespace Logitar.Portal.Core.Realms.Commands;

internal class DeleteRealmHandler : IRequestHandler<DeleteRealm, Realm>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IMediator _mediator;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public DeleteRealmHandler(IApplicationContext applicationContext,
    IMediator mediator,
    IRealmQuerier realmQuerier,
    IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _mediator = mediator;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm> Handle(DeleteRealm request, CancellationToken cancellationToken)
  {
    RealmAggregate realm = await _realmRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<RealmAggregate>(request.Id);
    Realm output = await _realmQuerier.GetAsync(realm, cancellationToken);

    await _mediator.Send(new DeleteSessions(realm), cancellationToken);
    await _mediator.Send(new DeleteUsers(realm), cancellationToken);

    await _mediator.Send(new DeleteDictionaries(realm), cancellationToken);
    await _mediator.Send(new DeleteSenders(realm), cancellationToken);
    await _mediator.Send(new DeleteTemplates(realm), cancellationToken);

    realm.Delete(_applicationContext.ActorId);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return output;
  }
}
