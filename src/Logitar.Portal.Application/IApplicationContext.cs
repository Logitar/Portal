using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application;

public interface IApplicationContext
{
  ConfigurationAggregate Configuration { get; set; }
}
