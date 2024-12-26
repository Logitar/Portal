using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application.Configurations;

public interface IConfigurationQuerier
{
  Task<ConfigurationModel> ReadAsync(Configuration configuration, CancellationToken cancellationToken = default);
}
