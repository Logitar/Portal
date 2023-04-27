using MediatR;

namespace Logitar.Portal.Core.Configurations.Queries;

internal class GetConfigurationHandler : IRequestHandler<GetConfiguration, Configuration?>
{
  private readonly IConfigurationRepository _configurationRepository;
  private readonly IMediator _mediator;

  public GetConfigurationHandler(IConfigurationRepository configurationRepository, IMediator mediator)
  {
    _configurationRepository = configurationRepository;
    _mediator = mediator;
  }

  public async Task<Configuration?> Handle(GetConfiguration request, CancellationToken cancellationToken)
  {
    ConfigurationAggregate? configuration = await _configurationRepository.LoadAsync(cancellationToken);

    return configuration == null
      ? null
      : await _mediator.Send(new ProjectToConfigurationOutput(configuration), cancellationToken);
  }
}
