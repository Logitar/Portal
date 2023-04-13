using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;
using MediatR;

namespace Logitar.Portal.Core.Configurations.Queries;

internal class IsConfigurationInitializedHandler : IRequestHandler<IsConfigurationInitialized, bool>
{
  private readonly IRealmRepository _realmRepository;
  private readonly IUserRepository _userRepository;

  public IsConfigurationInitializedHandler(IRealmRepository realmRepository, IUserRepository userRepository)
  {
    _realmRepository = realmRepository;
    _userRepository = userRepository;
  }

  public async Task<bool> Handle(IsConfigurationInitialized request, CancellationToken cancellationToken)
  {
    RealmAggregate? realm = await _realmRepository.LoadByUniqueNameAsync(RealmAggregate.PortalUniqueName, cancellationToken);
    if (realm == null)
    {
      return false;
    }

    IEnumerable<UserAggregate> users = await _userRepository.LoadAsync(realm, cancellationToken);

    return users.Any();
  }
}
