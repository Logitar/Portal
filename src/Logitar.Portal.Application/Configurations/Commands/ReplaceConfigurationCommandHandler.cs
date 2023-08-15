using AutoMapper;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal class ReplaceConfigurationCommandHandler : IRequestHandler<ReplaceConfigurationCommand, Configuration>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IMapper _mapper;

  public ReplaceConfigurationCommandHandler(IApplicationContext applicationContext,
    IConfigurationRepository configurationRepository, IMapper mapper)
  {
    _applicationContext = applicationContext;
    _configurationRepository = configurationRepository;
    _mapper = mapper;
  }

  public async Task<Configuration> Handle(ReplaceConfigurationCommand command, CancellationToken cancellationToken)
  {
    ConfigurationAggregate configuration = await _configurationRepository.LoadAsync(cancellationToken)
      ?? throw new NotImplementedException(); // TODO(fpion): implement

    ConfigurationAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _configurationRepository.LoadAsync(command.Version.Value, cancellationToken);
    }

    ReplaceConfigurationPayload payload = command.Payload;

    CultureInfo defaultLocale = payload.DefaultLocale.GetCultureInfo(nameof(payload.DefaultLocale))
      ?? CultureInfo.InvariantCulture;
    if (reference == null || defaultLocale != reference.DefaultLocale)
    {
      configuration.DefaultLocale = defaultLocale;
    }
    if (reference == null || payload.Secret != reference.Secret)
    {
      configuration.Secret = payload.Secret;
    }

    ReadOnlyUniqueNameSettings uniqueNameSettings = payload.UniqueNameSettings.ToReadOnlyUniqueNameSettings();
    if (reference == null || uniqueNameSettings != reference.UniqueNameSettings)
    {
      configuration.UniqueNameSettings = uniqueNameSettings;
    }
    ReadOnlyPasswordSettings passwordSettings = payload.PasswordSettings.ToReadOnlyPasswordSettings();
    if (reference == null || passwordSettings != reference.PasswordSettings)
    {
      configuration.PasswordSettings = passwordSettings;
    }

    ReadOnlyLoggingSettings loggingSettings = payload.LoggingSettings.ToReadOnlyLoggingSettings();
    if (reference == null || loggingSettings != reference.LoggingSettings)
    {
      configuration.LoggingSettings = loggingSettings;
    }

    if (configuration.HasChanges)
    {
      configuration.Update(_applicationContext.ActorId);

      await _configurationRepository.SaveAsync(configuration, cancellationToken);
    }

    Configuration result = _mapper.Map<Configuration>(configuration);
    // TODO(fpion): CreatedBy
    // TODO(fpion): ReplacedBy

    return result;
  }
}
