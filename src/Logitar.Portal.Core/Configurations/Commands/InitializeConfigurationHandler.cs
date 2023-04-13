using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Contact;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Configurations.Commands;

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
    InitializeConfigurationInput input = request.Input;
    CultureInfo defaultLocale = input.DefaultLocale.GetRequiredCultureInfo(nameof(input.DefaultLocale));

    RealmAggregate? realm = await _realmRepository.LoadByUniqueNameAsync(Constants.PortalRealm.UniqueName, cancellationToken);
    if (realm == null)
    {
      ReadOnlyUsernameSettings? usernameSettings = ReadOnlyUsernameSettings.From(input.UsernameSettings);
      ReadOnlyPasswordSettings? passwordSettings = ReadOnlyPasswordSettings.From(input.PasswordSettings);

      Dictionary<string, string> customAttributes = new(capacity: 1);
      if (input.LoggingSettings != null)
      {
        customAttributes[nameof(LoggingSettings)] = input.LoggingSettings.Serialize();
      }

      realm = new(_currentActor.Id, Constants.PortalRealm.UniqueName, Constants.PortalRealm.DisplayName,
        Constants.PortalRealm.Description, defaultLocale, usernameSettings: usernameSettings,
        passwordSettings: passwordSettings, customAttributes: customAttributes);

      await _realmRepository.SaveAsync(realm, cancellationToken);
    }

    InitialUserInput userInput = input.User;
    UserAggregate user = new(_currentActor.Id, realm, userInput.Username, userInput.FirstName,
      lastName: userInput.LastName, locale: defaultLocale);
    user.ChangePassword(_currentActor.Id, realm, userInput.Password);
    user.SetEmail(_currentActor.Id, new ReadOnlyEmail(userInput.EmailAddress));

    await _userRepository.SaveAsync(user, cancellationToken);
  }
}
