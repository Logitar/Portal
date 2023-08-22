using Logitar.EventSourcing;
using Logitar.Identity.Domain.Sessions;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Commands;

internal class DeleteRealmCommandHandler : IRequestHandler<DeleteRealmCommand, Realm?>
{
  private readonly IAggregateRepository _aggregateRepository;
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmQuerier _realmQuerier;
  private readonly IRealmRepository _realmRepository;
  private readonly ISessionRepository _sessionRepository;
  private readonly IUserRepository _userRepository;

  public DeleteRealmCommandHandler(IAggregateRepository aggregateRepository,
    IApplicationContext applicationContext, IRealmQuerier realmQuerier,
    IRealmRepository realmRepository, ISessionRepository sessionRepository,
    IUserRepository userRepository)
  {
    _aggregateRepository = aggregateRepository;
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
    _sessionRepository = sessionRepository;
    _userRepository = userRepository;
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

    realm.Delete(_applicationContext.ActorId);

    await DeleteSessionsAsync(realm, cancellationToken);
    await DeleteUsersAsync(realm, cancellationToken);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return result;
  }

  private async Task DeleteSessionsAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    IEnumerable<SessionAggregate> sessions = await _sessionRepository.LoadAsync(tenantId: realm.Id.Value, cancellationToken);
    foreach (SessionAggregate session in sessions)
    {
      session.Delete(_applicationContext.ActorId);
    }
    await _aggregateRepository.SaveAsync(sessions, cancellationToken);
  }
  private async Task DeleteUsersAsync(RealmAggregate realm, CancellationToken cancellationToken)
  {
    IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(tenantId: realm.Id.Value, cancellationToken);
    foreach (UserAggregate user in users)
    {
      user.Delete(_applicationContext.ActorId);
    }
    await _aggregateRepository.SaveAsync(users, cancellationToken);
  }
}
