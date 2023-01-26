using Logitar.Portal.Core2.Configurations.Payloads;

namespace Logitar.Portal.Core2.Configurations
{
  internal interface IConfigurationService
  {
    Task InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken = default);
    Task<bool> IsInitializedAsync(CancellationToken cancellationToken = default);
  }
}
