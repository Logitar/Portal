using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Configurations.Queries;
using Logitar.Portal.Contracts.Configurations;
using MediatR;

namespace Logitar.Portal.Application.Configurations;

internal class ConfigurationFacade : IConfigurationService
{
  private readonly IMediator _mediator;

  public ConfigurationFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Configuration> ReadAsync(CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadConfigurationQuery(), cancellationToken);
  }

  public async Task<Configuration> ReplaceAsync(ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReplaceConfigurationCommand(payload, version), cancellationToken);
  }

  public async Task<Configuration> UpdateAsync(UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new UpdateConfigurationCommand(payload), cancellationToken);
  }
}
