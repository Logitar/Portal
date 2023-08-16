using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application.Caching;

public interface ICacheService
{
  ConfigurationAggregate? Configuration { get; set; }
}
