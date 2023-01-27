using FluentValidation;
using Logitar.Portal.Core.Configurations.Payloads;
using Logitar.Portal.Core.Users;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Configurations.Commands
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
      Configuration? configuration = await _repository.LoadAsync<Configuration>(Configuration.AggregateId, cancellationToken);
      if (configuration != null)
      {
        throw new ConfigurationAlreadyInitializedException();
      }

      InitializeConfigurationPayload payload = request.Payload;
      UserPayload userPayload = payload.User;

      AggregateId userId = AggregateId.NewId();

      CultureInfo defaultLocale = CultureInfo.GetCultureInfo(payload.DefaultLocale);
      configuration = new(defaultLocale, payload.JwtSecret, payload.UsernameSettings, payload.PasswordSettings, userId);
      _configurationValidator.ValidateAndThrow(configuration);

      _passwordService.ValidateAndThrow(userPayload.Password, configuration);
      string passwordHash = _passwordService.Hash(userPayload.Password);
      CultureInfo locale = CultureInfo.GetCultureInfo(userPayload.Locale);
      User user = new(userId, userPayload.Username, passwordHash, userPayload.Email, userPayload.FirstName, userPayload.LastName, locale);
      _userValidator.ValidateAndThrow(user, configuration);

      await _repository.SaveAsync(new AggregateRoot[] { configuration, user }, cancellationToken);

      return Unit.Value;
    }
  }
}
