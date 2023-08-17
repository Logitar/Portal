using AutoMapper;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations.Commands;

internal class UpdateConfigurationCommandHandler : IRequestHandler<UpdateConfigurationCommand, Configuration>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IMapper _mapper;

  public UpdateConfigurationCommandHandler(IApplicationContext applicationContext,
    IConfigurationRepository configurationRepository, IMapper mapper)
  {
    _applicationContext = applicationContext;
    _configurationRepository = configurationRepository;
    _mapper = mapper;
  }

  public async Task<Configuration> Handle(UpdateConfigurationCommand command, CancellationToken cancellationToken)
  {
    ConfigurationAggregate configuration = await _configurationRepository.LoadAsync(cancellationToken)
      ?? throw new InvalidOperationException("The configuration could not be found.");

    UpdateConfigurationPayload payload = command.Payload;

    if (payload.DefaultLocale != null)
    {
      CultureInfo defaultLocale = payload.DefaultLocale.GetCultureInfo(nameof(payload.DefaultLocale))
        ?? CultureInfo.InvariantCulture;
      configuration.DefaultLocale = defaultLocale;
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

    if (configuration.HasChanges)
    {
      configuration.Update(_applicationContext.ActorId);

      await _configurationRepository.SaveAsync(configuration, cancellationToken);
    }

    Configuration result = _mapper.Map<Configuration>(configuration);

    return result;
  }
}
