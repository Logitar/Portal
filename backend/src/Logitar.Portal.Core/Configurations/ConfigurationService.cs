using Logitar.Portal.Core.Configurations.Commands;
using Logitar.Portal.Core.Configurations.Payloads;
using Logitar.Portal.Core.Configurations.Queries;

namespace Logitar.Portal.Core.Configurations
{
  internal class ConfigurationService : IConfigurationService
  {
    private readonly IRequestPipeline _requestPipeline;

    public ConfigurationService(IRequestPipeline requestPipeline)
    {
      _requestPipeline = requestPipeline;
    }

    public async Task InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken)
    {
      await _requestPipeline.ExecuteAsync(new InitializeConfigurationCommand(payload), cancellationToken);
    }

    public async Task<bool> IsInitializedAsync(CancellationToken cancellationToken)
    {
      return await _requestPipeline.ExecuteAsync(new IsConfigurationInitializedQuery(), cancellationToken);
    }
  }
}
