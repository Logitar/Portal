using Logitar.Portal.Core.Configurations.Commands;
using Logitar.Portal.Core.Configurations.Queries;

namespace Logitar.Portal.Core.Configurations;

internal class ConfigurationService : IConfigurationService
{
  private readonly IRequestPipeline _pipeline;

  public ConfigurationService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task InitializeAsync(InitializeConfigurationInput input, CancellationToken cancellationToken)
  {
    await _pipeline.ExecuteAsync(new InitializeConfiguration(input), cancellationToken);
  }

  public async Task<bool> IsInitializedAsync(CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new IsConfigurationInitialized(), cancellationToken);
  }
}
