using AutoMapper;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;
using Logitar.Portal.Core.Users.Contact;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Configurations.Commands;

internal class InitializeConfigurationHandler : IRequestHandler<InitializeConfiguration, Configuration>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IMapper _mapper;
  private readonly IUserRepository _userRepository;

  public InitializeConfigurationHandler(IApplicationContext applicationContext,
    IConfigurationRepository configurationRepository,
    IMapper mapper,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _configurationRepository = configurationRepository;
    _mapper = mapper;
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

    // TODO(fpion): user should create itself and the configuration

    ConfigurationAggregate configuration = new(_applicationContext.ActorId, defaultLocale, input.Secret,
      usernameSettings, passwordSettings, loggingSettings);

    UserAggregate user = new(_applicationContext.ActorId, configuration.UsernameSettings, userInput.Username, userInput.FirstName,
      lastName: userInput.LastName, locale: defaultLocale);
    user.ChangePassword(_applicationContext.ActorId, configuration.PasswordSettings, userInput.Password);
    user.SetEmail(_applicationContext.ActorId, new ReadOnlyEmail(userInput.EmailAddress));

    await _configurationRepository.SaveAsync(configuration, cancellationToken);
    await _userRepository.SaveAsync(user, cancellationToken);

    return _mapper.Map<Configuration>(configuration);
  }
}
