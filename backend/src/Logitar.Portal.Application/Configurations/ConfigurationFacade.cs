using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Configurations.Queries;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Configurations;

internal class ConfigurationFacade : IConfigurationService
{
  private readonly IMediator _mediator;

  public ConfigurationFacade(IMediator mediator)
  {
    _mediator = mediator;
  }

  public async Task<Session> InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken)
  {
    return await _mediator.Send(new InitializeConfigurationCommand(payload), cancellationToken);
  }

  public async Task<Configuration?> ReadAsync(CancellationToken cancellationToken)
  {
    return await _mediator.Send(new ReadConfigurationQuery(), cancellationToken);
  }
}
