using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application;

public interface ICacheService
{
  ConfigurationAggregate? Configuration { get; set; }
}
