using Logitar.EventSourcing;

namespace Logitar.Portal.Domain.Configurations;

public record ConfigurationId
{
  public AggregateId AggregateId { get; }

  public ConfigurationId()
  {
    AggregateId = new(Guid.Empty);
  }
}
