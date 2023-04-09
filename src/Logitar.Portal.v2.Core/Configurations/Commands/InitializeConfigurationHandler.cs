using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Users;
using Logitar.Portal.v2.Core.Users.Contact;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.v2.Core.Configurations.Commands;

internal class InitializeConfigurationHandler : IRequestHandler<InitializeConfiguration>
{
  private readonly ICurrentActor _currentActor;
  private readonly IRealmRepository _realmRepository;
  private readonly IUserRepository _userRepository;

  public InitializeConfigurationHandler(
    ICurrentActor currentActor,
    IRealmRepository realmRepository,
    IUserRepository userRepository)
  {
    _currentActor = currentActor;
    _realmRepository = realmRepository;
    _userRepository = userRepository;
  }

  public async Task Handle(InitializeConfiguration request, CancellationToken cancellationToken)
  {
    RealmAggregate? realm = await _realmRepository.LoadByUniqueNameAsync(Constants.PortalRealm.UniqueName, cancellationToken);
    if (realm == null)
    {
      realm = new(_currentActor.Id, Constants.PortalRealm.UniqueName, Constants.PortalRealm.DisplayName,
        Constants.PortalRealm.Description, Constants.PortalRealm.DefaultLocale, url: request.Url);

      await _realmRepository.SaveAsync(realm, cancellationToken);
    }

    InitialUserInput input = request.Input.User;
    CultureInfo? locale = input.Locale.GetCultureInfo(nameof(input.Locale));

    UserAggregate user = new(_currentActor.Id, realm, input.Username,
      input.FirstName, lastName: input.LastName, locale: locale);
    user.ChangePassword(_currentActor.Id, realm, input.Password);
    user.SetEmail(_currentActor.Id, new ReadOnlyEmail(input.EmailAddress));

    await _userRepository.SaveAsync(user, cancellationToken);
  }
}
