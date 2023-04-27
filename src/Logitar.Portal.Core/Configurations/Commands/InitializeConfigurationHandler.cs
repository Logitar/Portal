using Logitar.EventSourcing;
using Logitar.Portal.Core.Configurations.Queries;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Contact;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Configurations.Commands;

internal class InitializeConfigurationHandler : IRequestHandler<InitializeConfiguration, Configuration>
{
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IMediator _mediator;
  private readonly IUserRepository _userRepository;

  public InitializeConfigurationHandler(IConfigurationRepository configurationRepository,
    IMediator mediator,
    IUserRepository userRepository)
  {
    _configurationRepository = configurationRepository;
    _mediator = mediator;
    _userRepository = userRepository;
  }

  public async Task<Configuration> Handle(InitializeConfiguration request, CancellationToken cancellationToken)
  {
    if (await _configurationRepository.LoadAsync(cancellationToken) != null)
    {
      throw new ConfigurationAlreadyInitializedException();
    }

    InitializeConfigurationInput input = request.Input;
    InitialUserInput userInput = input.User;

    CultureInfo defaultLocale = input.DefaultLocale.GetRequiredCultureInfo(nameof(input.DefaultLocale));
    ReadOnlyUsernameSettings? usernameSettings = ReadOnlyUsernameSettings.From(input.UsernameSettings);
    ReadOnlyPasswordSettings? passwordSettings = ReadOnlyPasswordSettings.From(input.PasswordSettings);
    ReadOnlyLoggingSettings? loggingSettings = ReadOnlyLoggingSettings.From(input.LoggingSettings);

    AggregateId userId = AggregateId.NewId();

    ConfigurationAggregate configuration = new(userId, defaultLocale, input.Secret,
      usernameSettings, passwordSettings, loggingSettings);

    UserAggregate user = new(userId, configuration.UsernameSettings, userInput.Username, userInput.FirstName,
      lastName: userInput.LastName, locale: defaultLocale, id: userId);
    user.ChangePassword(user.Id, configuration.PasswordSettings, userInput.Password);
    user.SetEmail(user.Id, new ReadOnlyEmail(userInput.EmailAddress));

    await _configurationRepository.SaveAsync(configuration, cancellationToken);
    await _userRepository.SaveAsync(user, cancellationToken);

    return await _mediator.Send(new ProjectToConfigurationOutput(configuration), cancellationToken);
  }
}
