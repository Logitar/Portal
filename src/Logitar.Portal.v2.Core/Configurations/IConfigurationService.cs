namespace Logitar.Portal.v2.Core.Configurations;

public interface IConfigurationService
{
  Task InitializeAsync(InitializeConfigurationInput input, CancellationToken cancellationToken = default);
  Task<bool> IsInitializedAsync(CancellationToken cancellationToken = default);
}
