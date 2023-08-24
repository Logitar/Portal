using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal class UpdateConfigurationCommandHandler : IRequestHandler<UpdateConfigurationCommand, Configuration>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IConfigurationQuerier _configurationQuerier;
  private readonly IConfigurationRepository _configurationRepository;

  public UpdateConfigurationCommandHandler(IApplicationContext applicationContext, IConfigurationQuerier configurationQuerier,
    IConfigurationRepository configurationRepository)
  {
    _applicationContext = applicationContext;
    _configurationQuerier = configurationQuerier;
    _configurationRepository = configurationRepository;
  }

  public async Task<Configuration> Handle(UpdateConfigurationCommand command, CancellationToken cancellationToken)
  {
    ConfigurationAggregate configuration = await _configurationRepository.LoadAsync(cancellationToken)
      ?? throw new InvalidOperationException("The configuration has not been initialized.");

    UpdateConfigurationPayload payload = command.Payload;

    if (payload.DefaultLocale != null)
    {
      configuration.DefaultLocale = payload.DefaultLocale.GetRequiredLocale(nameof(payload.DefaultLocale));
    }
    if (payload.Secret != null)
    {
      configuration.Secret = payload.Secret;
    }

    if (payload.UniqueNameSettings != null)
    {
      configuration.UniqueNameSettings = payload.UniqueNameSettings.ToReadOnlyUniqueNameSettings();
    }
    if (payload.PasswordSettings != null)
    {
      configuration.PasswordSettings = payload.PasswordSettings.ToReadOnlyPasswordSettings();
    }

    if (payload.LoggingSettings != null)
    {
      configuration.LoggingSettings = payload.LoggingSettings.ToReadOnlyLoggingSettings();
    }

    configuration.Update(_applicationContext.ActorId);

    await _configurationRepository.SaveAsync(configuration, cancellationToken);

    return await _configurationQuerier.ReadAsync(configuration, cancellationToken);
  }
}
