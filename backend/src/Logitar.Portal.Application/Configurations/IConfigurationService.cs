using Logitar.Portal.Application.Configurations.Payloads;

namespace Logitar.Portal.Application.Configurations
{
  public interface IConfigurationService
  {
    Task InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken = default);
    Task<bool> IsInitializedAsync(CancellationToken cancellationToken = default);
  }
}
