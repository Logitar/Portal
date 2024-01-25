using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application.Configurations;

public interface IConfigurationQuerier
{
  Task<Configuration?> ReadAsync(CancellationToken cancellationToken = default);
  Task<Configuration> ReadAsync(ConfigurationAggregate configuration, CancellationToken cancellationToken = default);
}
