using Logitar.EventSourcing;
using Logitar.Portal.v2.Contracts.Users;
using Logitar.Portal.v2.Core.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Commands;

internal class SetExternalIdentifierHandler : IRequestHandler<SetExternalIdentifier, User>
{
  private readonly ICurrentActor _currentActor;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SetExternalIdentifierHandler(ICurrentActor currentActor,
    IRealmRepository realmRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _realmRepository = realmRepository;
    _userQuerier = userQuerier;
    _userRepository = userRepository;
  }

  public async Task<User> Handle(SetExternalIdentifier request, CancellationToken cancellationToken)
  {
    UserAggregate user = await _userRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<UserAggregate>(request.Id);

    string key = request.Key.Trim();
    string? value = request.Value?.CleanTrim();

    if (value != null)
    {
      RealmAggregate? realm = user.RealmId.HasValue
        ? await _realmRepository.LoadAsync(user, cancellationToken)
        : null;

      if (await _userRepository.LoadAsync(realm, key, value, cancellationToken) != null)
      {
        throw new ExternalIdentifierAlreadyUsedException(realm, key, value);
      }
    }

    user.SetExternalIdentifier(_currentActor.Id, key, value);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
