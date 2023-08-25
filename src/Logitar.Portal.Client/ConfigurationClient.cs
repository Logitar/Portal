using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Contracts.Sessions;

namespace Logitar.Portal.Client;

internal class ConfigurationClient : IConfigurationService
{
  public Task<Session> InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken)
  {
    throw new NotSupportedException();
  }

  public Task<Configuration?> ReadAsync(CancellationToken cancellationToken)
  {
    throw new NotSupportedException();
  }

  public Task<Configuration> ReplaceAsync(ReplaceConfigurationPayload payload, long? version, CancellationToken cancellationToken)
  {
    throw new NotSupportedException();
  }

  public Task<Configuration> UpdateAsync(UpdateConfigurationPayload payload, CancellationToken cancellationToken)
  {
    throw new NotSupportedException();
  }
}
