using Logitar.EventSourcing;

namespace Logitar.Portal.Domain.Configurations;

public record ConfigurationId
{
  public AggregateId AggregateId { get; }
  public string Value => AggregateId.Value;

  public ConfigurationId()
  {
    AggregateId = new("Configuration");
  }
}
