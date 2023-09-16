using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Settings;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal class ReplaceConfigurationCommandHandler : IRequestHandler<ReplaceConfigurationCommand, Configuration>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;

  public ReplaceConfigurationCommandHandler(IApplicationContext applicationContext, IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository)
  {
    _applicationContext = applicationContext;
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
  }

  public async Task<Configuration> Handle(ReplaceConfigurationCommand command, CancellationToken cancellationToken)
  {
    ConfigurationAggregate configuration = await _configurationRepository.LoadAsync(cancellationToken)
      ?? throw new InvalidOperationException("The configuration has not been initialized.");

    ConfigurationAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _configurationRepository.LoadAsync(command.Version.Value, cancellationToken);
    }

    ReplaceConfigurationPayload payload = command.Payload;
    ReadOnlyLocale defaultLocale = payload.DefaultLocale.GetRequiredLocale(nameof(payload.DefaultLocale));
    ReadOnlyLoggingSettings loggingSettings = payload.LoggingSettings.ToReadOnlyLoggingSettings();
    ReadOnlyPasswordSettings passwordSettings = payload.PasswordSettings.ToReadOnlyPasswordSettings();
    ReadOnlyUniqueNameSettings uniqueNameSettings = payload.UniqueNameSettings.ToReadOnlyUniqueNameSettings();

    if (reference == null || defaultLocale != reference.DefaultLocale)
    {
      configuration.DefaultLocale = defaultLocale;
    }
    if (reference == null || payload.Secret.Trim() != reference.Secret)
    {
      configuration.Secret = payload.Secret;
    }

    if (reference == null || uniqueNameSettings != reference.UniqueNameSettings)
    {
      configuration.UniqueNameSettings = uniqueNameSettings;
    }
    if (reference == null || passwordSettings != reference.PasswordSettings)
    {
      configuration.PasswordSettings = passwordSettings;
    }

    if (reference == null || loggingSettings != reference.LoggingSettings)
    {
      configuration.LoggingSettings = loggingSettings;
    }

    configuration.Update(_applicationContext.ActorId);

    await _configurationRepository.SaveAsync(configuration, cancellationToken);

    return await _configurationQuerier.ReadAsync(configuration, cancellationToken);
  }
}
