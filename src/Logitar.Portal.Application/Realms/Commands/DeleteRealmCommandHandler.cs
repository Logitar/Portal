using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal class DeleteRealmCommandHandler : IRequestHandler<DeleteRealmCommand, Realm?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public DeleteRealmCommandHandler(IApplicationContext applicationContext,
    IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
  }

  public async Task<Realm?> Handle(DeleteRealmCommand command, CancellationToken cancellationToken)
  {
    AggregateId id = command.Id.GetAggregateId(nameof(command.Id));
    RealmAggregate? realm = await _realmRepository.LoadAsync(id, cancellationToken);
    if (realm == null)
    {
      return null;
    }
    Realm result = await _realmQuerier.ReadAsync(realm, cancellationToken);

    // TODO(fpion): Delete Messages?
    // TODO(fpion): Delete Templates
    // TODO(fpion): Delete Senders
    // TODO(fpion): Delete Dictionaries
    // TODO(fpion): Delete Sessions
    // TODO(fpion): Delete Users
    // TODO(fpion): Delete API Keys
    // TODO(fpion): Delete Roles
    realm.Delete(_applicationContext.ActorId);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return result;
  }
}
