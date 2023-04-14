using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Users;
using Logitar.Portal.Core.Realms;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal class SetExternalIdentifierHandler : IRequestHandler<SetExternalIdentifier, User>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserQuerier _userQuerier;
  private readonly IUserRepository _userRepository;

  public SetExternalIdentifierHandler(IApplicationContext applicationContext,
    IRealmRepository realmRepository,
    IUserQuerier userQuerier,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
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
      RealmAggregate? realm = await _realmRepository.LoadAsync(user, cancellationToken);
      if (await _userRepository.LoadAsync(realm, key, value, cancellationToken) != null)
      {
        throw new ExternalIdentifierAlreadyUsedException(realm, key, value);
      }
    }

    user.SetExternalIdentifier(_applicationContext.ActorId, key, value);

    await _userRepository.SaveAsync(user, cancellationToken);

    return await _userQuerier.GetAsync(user, cancellationToken);
  }
}
