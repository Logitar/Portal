using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Configurations.Validators;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal class UpdateConfigurationCommandHandler : IRequestHandler<UpdateConfigurationCommand, ConfigurationModel>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;

  public UpdateConfigurationCommandHandler(ICacheService cacheService, IConfigurationRepository configurationRepository)
  {
    _cacheService = cacheService;
    _configurationRepository = configurationRepository;
  }

  public async Task<ConfigurationModel> Handle(UpdateConfigurationCommand command, CancellationToken cancellationToken)
  {
    UpdateConfigurationPayload payload = command.Payload;
    new UpdateConfigurationValidator().ValidateAndThrow(payload);

    Configuration configuration = await _configurationRepository.LoadAsync(cancellationToken)
      ?? throw new InvalidOperationException("The configuration has not been initialized yet.");

    if (payload.DefaultLocale != null)
    {
      configuration.DefaultLocale = LocaleUnit.TryCreate(payload.DefaultLocale.Value);
    }
    if (payload.Secret != null)
    {
      configuration.Secret = JwtSecret.CreateOrGenerate(payload.Secret);
    }

    if (payload.UniqueNameSettings != null)
    {
      configuration.UniqueNameSettings = new ReadOnlyUniqueNameSettings(payload.UniqueNameSettings);
    }
    if (payload.PasswordSettings != null)
    {
      configuration.PasswordSettings = new ReadOnlyPasswordSettings(payload.PasswordSettings);
    }
    if (payload.RequireUniqueEmail.HasValue)
    {
      configuration.RequireUniqueEmail = payload.RequireUniqueEmail.Value;
    }

    if (payload.LoggingSettings != null)
    {
      configuration.LoggingSettings = new ReadOnlyLoggingSettings(payload.LoggingSettings);
    }

    configuration.Update(command.ActorId);
    await _configurationRepository.SaveAsync(configuration, cancellationToken);

    return _cacheService.Configuration ?? throw new InvalidOperationException("The configuration should be in the cache.");
  }
}
