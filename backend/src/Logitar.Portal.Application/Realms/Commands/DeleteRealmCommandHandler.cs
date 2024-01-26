using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal class DeleteRealmCommandHandler : IRequestHandler<DeleteRealmCommand, Realm?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public DeleteRealmCommandHandler(IApplicationContext applicationContext, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm?> Handle(DeleteRealmCommand command, CancellationToken cancellationToken)
  {
    RealmId realmId = new(command.Id);
    RealmAggregate? realm = await _realmRepository.LoadAsync(realmId, cancellationToken);
    if (realm == null)
    {
      return null;
    }

    Realm result = await _realmQuerier.ReadAsync(realm, cancellationToken);

    realm.Delete(_applicationContext.ActorId);

    await _realmRepository.SaveAsync(realm, cancellationToken); // TODO(fpion): deleting a realm should delete all its aggregates

    return result;
  }
}
