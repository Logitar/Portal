using Logitar.Portal.Core.Configurations.Queries;
using Logitar.Portal.Core.Realms;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Configurations.Commands;

internal class UpdateConfigurationHandler : IRequestHandler<UpdateConfiguration, Configuration>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IMediator _mediator;

  public UpdateConfigurationHandler(IApplicationContext applicationContext,
    IConfigurationRepository configurationRepository,
    IMediator mediator)
  {
    _applicationContext = applicationContext;
    _configurationRepository = configurationRepository;
    _mediator = mediator;
  }

  public async Task<Configuration> Handle(UpdateConfiguration request, CancellationToken cancellationToken)
  {
    UpdateConfigurationInput input = request.Input;

    CultureInfo defaultLocale = input.DefaultLocale.GetRequiredCultureInfo(nameof(input.DefaultLocale));
    ReadOnlyUsernameSettings? usernameSettings = ReadOnlyUsernameSettings.From(input.UsernameSettings);
    ReadOnlyPasswordSettings? passwordSettings = ReadOnlyPasswordSettings.From(input.PasswordSettings);
    ReadOnlyLoggingSettings? loggingSettings = ReadOnlyLoggingSettings.From(input.LoggingSettings);

    ConfigurationAggregate configuration = _applicationContext.Configuration;
    configuration.Update(_applicationContext.ActorId, defaultLocale, input.Secret,
      usernameSettings, passwordSettings, loggingSettings);

    await _configurationRepository.SaveAsync(configuration, cancellationToken);

    return await _mediator.Send(new ProjectToConfigurationOutput(configuration), cancellationToken);
  }
}
