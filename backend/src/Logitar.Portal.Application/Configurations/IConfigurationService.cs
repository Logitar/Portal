using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Application.Configurations;

public interface IConfigurationService
{
  Task<Session> InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken = default);
  Task<Configuration?> ReadAsync(CancellationToken cancellationToken = default);
  Task<Configuration> ReplaceAsync(ReplaceConfigurationPayload payload, long? version = null, CancellationToken cancellationToken = default);
  Task<Configuration> UpdateAsync(UpdateConfigurationPayload payload, CancellationToken cancellationToken = default);
}
