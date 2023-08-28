using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Configurations.Queries;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Application.Configurations;

internal class ConfigurationService : IConfigurationService
{
  private readonly IRequestPipeline _pipeline;

  public ConfigurationService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task<InitializeConfigurationResult> InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new InitializeConfigurationCommand(payload), cancellationToken);
  }

  public async Task<Configuration?> ReadAsync(CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReadConfigurationQuery(), cancellationToken);
  }

  public async Task<Configuration> ReplaceAsync(ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new ReplaceConfigurationCommand(payload, version), cancellationToken);
  }

  public async Task<Configuration> UpdateAsync(UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new UpdateConfigurationCommand(payload), cancellationToken);
  }
}
