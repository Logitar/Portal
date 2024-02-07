using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal class DeleteRealmCommandHandler : IRequestHandler<DeleteRealmCommand, Realm?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmManager _realmManager;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public DeleteRealmCommandHandler(IApplicationContext applicationContext,
    IRealmManager realmManager, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _realmManager = realmManager;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm?> Handle(DeleteRealmCommand command, CancellationToken cancellationToken)
  {
    RealmAggregate? realm = await _realmRepository.LoadAsync(command.Id, cancellationToken);
    if (realm == null)
    {
      return null;
    }
    Realm result = await _realmQuerier.ReadAsync(realm, cancellationToken);

    ActorId actorId = _applicationContext.ActorId;
    realm.Delete(actorId);
    await _realmManager.SaveAsync(realm, actorId, cancellationToken);

    return result;
  }
}
