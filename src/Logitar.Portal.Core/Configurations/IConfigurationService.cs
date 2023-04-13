namespace Logitar.Portal.Core.Configurations;

public interface IConfigurationService
{
  Task InitializeAsync(InitializeConfigurationInput input, CancellationToken cancellationToken = default);
  Task<bool> IsInitializedAsync(CancellationToken cancellationToken = default);
}
