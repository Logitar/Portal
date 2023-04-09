using Logitar.Portal.v2.Core.Configurations.Commands;
using Logitar.Portal.v2.Core.Configurations.Queries;

namespace Logitar.Portal.v2.Core.Configurations;

internal class ConfigurationService : IConfigurationService
{
  private readonly IRequestPipeline _pipeline;

  public ConfigurationService(IRequestPipeline pipeline)
  {
    _pipeline = pipeline;
  }

  public async Task InitializeAsync(InitializeConfigurationInput input, Uri? url, CancellationToken cancellationToken)
  {
    await _pipeline.ExecuteAsync(new InitializeConfiguration(input, url), cancellationToken);
  }

  public async Task<bool> IsInitializedAsync(CancellationToken cancellationToken)
  {
    return await _pipeline.ExecuteAsync(new IsConfigurationInitialized(), cancellationToken);
  }
}
