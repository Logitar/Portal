using Logitar.Portal.Core.Configurations.Payloads;

namespace Logitar.Portal.Core.Configurations
{
  public interface IConfigurationService
  {
    Task InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken = default);
    Task<bool> IsInitializedAsync(CancellationToken cancellationToken = default);
  }
}
