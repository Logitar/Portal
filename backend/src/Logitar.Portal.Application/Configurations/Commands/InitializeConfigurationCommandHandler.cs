using FluentValidation;
using Logitar.Portal.Application.Configurations.Payloads;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Application.Configurations.Commands
{
  internal class InitializeConfigurationCommandHandler : IRequestHandler<InitializeConfigurationCommand>
  {
    private readonly IValidator<Configuration> _configurationValidator;
    private readonly IPasswordService _passwordService;
    private readonly IRepository _repository;
    private readonly IUserValidator _userValidator;

    public InitializeConfigurationCommandHandler(IValidator<Configuration> configurationValidator,
      IPasswordService passwordService,
      IRepository repository,
      IUserValidator userValidator)
    {
      _configurationValidator = configurationValidator;
      _passwordService = passwordService;
      _repository = repository;
      _userValidator = userValidator;
    }

    public async Task<Unit> Handle(InitializeConfigurationCommand request, CancellationToken cancellationToken)
    {
      if (await _repository.LoadConfigurationAsync(cancellationToken) != null)
      {
        throw new ConfigurationAlreadyInitializedException();
      }

      InitializeConfigurationPayload payload = request.Payload;
      UserPayload userPayload = payload.User;

      CultureInfo defaultLocale = CultureInfo.GetCultureInfo(payload.DefaultLocale);
      UsernameSettings usernameSettings = new()
      {
        AllowedCharacters = payload.UsernameSettings.AllowedCharacters
      };
      PasswordSettings passwordSettings = new()
      {
        RequiredLength = payload.PasswordSettings.RequiredLength,
        RequiredUniqueChars = payload.PasswordSettings.RequiredUniqueChars,
        RequireNonAlphanumeric = payload.PasswordSettings.RequireNonAlphanumeric,
        RequireLowercase = payload.PasswordSettings.RequireLowercase,
        RequireUppercase = payload.PasswordSettings.RequireUppercase,
        RequireDigit = payload.PasswordSettings.RequireDigit
      };

      _passwordService.ValidateAndThrow(userPayload.Password, passwordSettings);
      string passwordHash = _passwordService.Hash(userPayload.Password);
      User user = new(userPayload.Username, passwordHash, userPayload.Email, userPayload.FirstName, userPayload.LastName, defaultLocale);
      _userValidator.ValidateAndThrow(user, usernameSettings);

      Configuration configuration = new(defaultLocale, payload.JwtSecret, usernameSettings, passwordSettings, user.Id);
      _configurationValidator.ValidateAndThrow(configuration);

      await _repository.SaveAsync(new AggregateRoot[] { configuration, user }, cancellationToken);

      return Unit.Value;
    }
  }
}
