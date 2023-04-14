namespace Logitar.Portal.Core.Configurations;

public interface IConfigurationService
{
  Task<Configuration> InitializeAsync(InitializeConfigurationInput input, CancellationToken cancellationToken = default);
  Task<bool> IsInitializedAsync(CancellationToken cancellationToken = default);
}
