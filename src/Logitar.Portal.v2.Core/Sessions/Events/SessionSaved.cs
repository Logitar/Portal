using Logitar.EventSourcing;

namespace Logitar.Portal.v2.Core.Sessions.Events;

public abstract record SessionSaved : DomainEvent
{
  public string? IpAddress { get; init; }
  public string? AdditionalInformation { get; init; }

  public Dictionary<string, string> CustomAttributes { get; init; } = new();
}
