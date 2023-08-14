namespace Logitar.Portal.Application.Configurations;

public interface IConfigurationService
{
  Task InitializeAsync(InitializeConfigurationPayload payload, CancellationToken cancellationToken = default);
  Task<Configuration> ReadAsync(CancellationToken cancellationToken = default);
}
