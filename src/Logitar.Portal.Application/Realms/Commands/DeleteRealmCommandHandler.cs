using Logitar.Portal.Application.ApiKeys.Commands;
using Logitar.Portal.Application.Dictionaries.Commands;
using Logitar.Portal.Application.Roles.Commands;
using Logitar.Portal.Application.Senders.Commands;
using Logitar.Portal.Application.Sessions.Commands;
using Logitar.Portal.Application.Users.Commands;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal class DeleteRealmCommandHandler : IRequestHandler<DeleteRealmCommand, Realm?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IPublisher _publisher;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;

  public DeleteRealmCommandHandler(IApplicationContext applicationContext, IPublisher publisher, IRealmQuerier realmQuerier, IRealmRepository realmRepository)
  {
    _applicationContext = applicationContext;
    _publisher = publisher;
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

    await _publisher.Publish(new DeleteSendersCommand(realm), cancellationToken);
    await _publisher.Publish(new DeleteDictionariesCommand(realm), cancellationToken);
    await _publisher.Publish(new DeleteSessionsCommand(realm), cancellationToken);
    await _publisher.Publish(new DeleteUsersCommand(realm), cancellationToken);
    await _publisher.Publish(new DeleteApiKeysCommand(realm), cancellationToken);
    await _publisher.Publish(new DeleteRolesCommand(realm), cancellationToken);

    realm.Delete(_applicationContext.ActorId);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return result;
  }
}
