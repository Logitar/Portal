using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Caching;
using Logitar.Portal.Application.Configurations.Validators;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal record ReplaceConfigurationCommand(ReplaceConfigurationPayload Payload, long? Version) : Activity, IRequest<ConfigurationModel>;

internal class ReplaceConfigurationCommandHandler : IRequestHandler<ReplaceConfigurationCommand, ConfigurationModel>
{
  private readonly ICacheService _cacheService;
  private readonly IConfigurationRepository _configurationRepository;

  public ReplaceConfigurationCommandHandler(ICacheService cacheService, IConfigurationRepository configurationRepository)
  {
    _cacheService = cacheService;
    _configurationRepository = configurationRepository;
  }

  public async Task<ConfigurationModel> Handle(ReplaceConfigurationCommand command, CancellationToken cancellationToken)
  {
    ReplaceConfigurationPayload payload = command.Payload;
    new ReplaceConfigurationValidator().ValidateAndThrow(payload);

    Configuration configuration = await _configurationRepository.LoadAsync(cancellationToken)
      ?? throw new InvalidOperationException("The configuration has not been initialized yet.");
    Configuration? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _configurationRepository.LoadAsync(command.Version.Value, cancellationToken);
    }

    LocaleUnit? defaultLocale = LocaleUnit.TryCreate(payload.DefaultLocale);
    if (reference == null || defaultLocale != reference.DefaultLocale)
    {
      configuration.DefaultLocale = defaultLocale;
    }
    JwtSecret secret = JwtSecret.CreateOrGenerate(payload.Secret);
    if (reference == null || secret != reference.Secret)
    {
      configuration.Secret = secret;
    }

    ReadOnlyUniqueNameSettings uniqueNameSettings = new(payload.UniqueNameSettings);
    if (reference == null || uniqueNameSettings != reference.UniqueNameSettings)
    {
      configuration.UniqueNameSettings = uniqueNameSettings;
    }
    ReadOnlyPasswordSettings passwordSettings = new(payload.PasswordSettings);
    if (reference == null || passwordSettings != reference.PasswordSettings)
    {
      configuration.PasswordSettings = passwordSettings;
    }
    if (reference == null || payload.RequireUniqueEmail != reference.RequireUniqueEmail)
    {
      configuration.RequireUniqueEmail = payload.RequireUniqueEmail;
    }

    ReadOnlyLoggingSettings loggingSettings = new(payload.LoggingSettings);
    if (reference == null || loggingSettings != reference.LoggingSettings)
    {
      configuration.LoggingSettings = loggingSettings;
    }

    configuration.Update(command.ActorId);
    await _configurationRepository.SaveAsync(configuration, cancellationToken);

    return _cacheService.Configuration ?? throw new InvalidOperationException("The configuration should be in the cache.");
  }
}
