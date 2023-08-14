using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Configurations.Queries;

namespace Logitar.Portal.Application.Configurations;

internal class ConfigurationService : IConfigurationService
{
  private readonly IRequestPipeline _pipeline;

  public ConfigurationService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken)
  {
    await _pipeline.ExecuteAsync(new InitializeConfigurationCommand(payload), cancellationToken);
  }

  public async Task<Configuration> ReadAsync(CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReadConfigurationQuery(), cancellationToken);
  }
}
