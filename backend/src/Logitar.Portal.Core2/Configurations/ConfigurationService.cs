using Logitar.Portal.Core2.Configurations.Commands;
using Logitar.Portal.Core2.Configurations.Payloads;
using Logitar.Portal.Core2.Configurations.Queries;

namespace Logitar.Portal.Core2.Configurations
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
