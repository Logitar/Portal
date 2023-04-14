﻿using Logitar.Portal.Contracts.Realms;
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
  private readonly IUserRepository _userRepository;

  public InitializeConfigurationHandler(IApplicationContext applicationContext,
    IConfigurationRepository configurationRepository,
    IUserRepository userRepository)
  {
    _applicationContext = applicationContext;
    _configurationRepository = configurationRepository;
    _userRepository = userRepository;
  }

  public async Task<Configuration> Handle(InitializeConfiguration request, CancellationToken cancellationToken)
  {
    InitializeConfigurationInput input = request.Input;

    CultureInfo defaultLocale = input.DefaultLocale.GetRequiredCultureInfo(nameof(input.DefaultLocale));
    ReadOnlyUsernameSettings? usernameSettings = ReadOnlyUsernameSettings.From(input.UsernameSettings);
    ReadOnlyPasswordSettings? passwordSettings = ReadOnlyPasswordSettings.From(input.PasswordSettings);
    ReadOnlyLoggingSettings? loggingSettings = ReadOnlyLoggingSettings.From(input.LoggingSettings);

    ConfigurationAggregate configuration = new(_applicationContext.ActorId, defaultLocale, input.Secret,
      usernameSettings, passwordSettings, loggingSettings);

    await _configurationRepository.SaveAsync(configuration, cancellationToken);

    InitialUserInput userInput = input.User;
    UserAggregate user = new(_applicationContext.ActorId, configuration.UsernameSettings, userInput.Username, userInput.FirstName,
      lastName: userInput.LastName, locale: defaultLocale);
    user.ChangePassword(_applicationContext.ActorId, configuration.PasswordSettings, userInput.Password);
    user.SetEmail(_applicationContext.ActorId, new ReadOnlyEmail(userInput.EmailAddress));

    await _userRepository.SaveAsync(user, cancellationToken);

    return new Configuration
    {
      DefaultLocale = configuration.DefaultLocale.Name,
      UsernameSettings = new UsernameSettings
      {
        AllowedCharacters = configuration.UsernameSettings.AllowedCharacters
      },
      PasswordSettings = new PasswordSettings
      {
        RequiredLength = configuration.PasswordSettings.RequiredLength,
        RequiredUniqueChars = configuration.PasswordSettings.RequiredUniqueChars,
        RequireNonAlphanumeric = configuration.PasswordSettings.RequireNonAlphanumeric,
        RequireLowercase = configuration.PasswordSettings.RequireLowercase,
        RequireUppercase = configuration.PasswordSettings.RequireUppercase,
        RequireDigit = configuration.PasswordSettings.RequireDigit
      },
      LoggingSettings = new LoggingSettings
      {
        Extent = configuration.LoggingSettings.Extent,
        OnlyErrors = configuration.LoggingSettings.OnlyErrors
      }
    };
  }
}
