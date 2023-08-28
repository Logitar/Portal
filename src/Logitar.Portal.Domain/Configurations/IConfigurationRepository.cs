namespace Logitar.Portal.Domain.Configurations;

public interface IConfigurationRepository
{
  Task<ConfigurationAggregate?> LoadAsync(CancellationToken cancellationToken = default);
  Task<ConfigurationAggregate?> LoadAsync(long? version, CancellationToken cancellationToken = default);
  Task SaveAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken = default);
}
