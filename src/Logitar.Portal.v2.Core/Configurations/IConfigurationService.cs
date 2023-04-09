namespace Logitar.Portal.v2.Core.Configurations;

public interface IConfigurationService
{
  Task InitializeAsync(InitializeConfigurationInput input, Uri? url = null, CancellationToken cancellationToken = default);
  Task<bool> IsInitializedAsync(CancellationToken cancellationToken = default);
}
