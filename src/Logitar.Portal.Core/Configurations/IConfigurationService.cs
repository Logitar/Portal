namespace Logitar.Portal.Core.Configurations;

public interface IConfigurationService
{
  Task<Configuration?> GetAsync(CancellationToken cancellationToken = default);
  Task<Configuration> InitializeAsync(InitializeConfigurationInput input, CancellationToken cancellationToken = default);
}
