using Logitar.EventSourcing;
using Logitar.Portal.Application;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal;

internal class TestApplicationContext : IApplicationContext
{
  public ActorId ActorId { get; set; } = new(Guid.Empty);

  private ConfigurationAggregate? _configuration = null;
  public ConfigurationAggregate Configuration
  {
    get => _configuration ?? throw new InvalidOperationException("The configuration has not been set.");
    set => _configuration = value;
  }
}
