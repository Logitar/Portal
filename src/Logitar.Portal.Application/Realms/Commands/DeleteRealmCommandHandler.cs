using Logitar.EventSourcing;
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
  private readonly IUserRepository _userRepository;

  public DeleteRealmCommandHandler(IAggregateRepository aggregateRepository,
    IApplicationContext applicationContext, IRealmQuerier realmQuerier,
    IRealmRepository realmRepository, IUserRepository userRepository)
  {
    _aggregateRepository = aggregateRepository;
    _applicationContext = applicationContext;
    _realmQuerier = realmQuerier;
    _realmRepository = realmRepository;
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

    // TODO(fpion): Delete Messages
    // TODO(fpion): Delete Templates
    // TODO(fpion): Delete Senders
    // TODO(fpion): Delete Dictionaries
    // TODO(fpion): Delete Sessions
    // TODO(fpion): Delete API Keys
    // TODO(fpion): Delete Roles

    IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(realm.Id.Value, cancellationToken);
    foreach (UserAggregate user in users)
    {
      user.Delete(_applicationContext.ActorId);
    }
    await _aggregateRepository.SaveAsync(users, cancellationToken);

    realm.Delete(_applicationContext.ActorId);

    await _realmRepository.SaveAsync(realm, cancellationToken);

    return result;
  }
}
