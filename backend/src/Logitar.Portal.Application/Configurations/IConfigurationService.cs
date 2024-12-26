using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Application.Configurations;

public interface IConfigurationService
{
  Task<ConfigurationModel> ReadAsync(CancellationToken cancellationToken = default);
  Task<ConfigurationModel> ReplaceAsync(ReplaceConfigurationPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<ConfigurationModel> UpdateAsync(UpdateConfigurationPayload payload, CancellationToken cancellationToken = default);
}
