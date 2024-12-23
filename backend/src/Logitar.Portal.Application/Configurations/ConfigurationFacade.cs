using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Configurations.Commands;
using Logitar.Portal.Application.Configurations.Queries;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Application.Configurations;

internal class ConfigurationFacade : IConfigurationService
{
  private readonly IActivityPipeline _activityPipeline;

  public ConfigurationFacade(IActivityPipeline activityPipeline)
  {
    _activityPipeline = activityPipeline;
  }

  public async Task<ConfigurationModel> ReadAsync(CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReadConfigurationQuery(), cancellationToken);
  }

  public async Task<ConfigurationModel> ReplaceAsync(ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new ReplaceConfigurationCommand(payload, version), cancellationToken);
  }

  public async Task<ConfigurationModel> UpdateAsync(UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    return await _activityPipeline.ExecuteAsync(new UpdateConfigurationCommand(payload), cancellationToken);
  }
}
