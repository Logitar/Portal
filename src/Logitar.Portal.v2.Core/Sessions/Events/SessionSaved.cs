using Logitar.EventSourcing;

namespace Logitar.Portal.v2.Core.Sessions.Events;

public abstract record SessionSaved : DomainEvent
{
  public Dictionary<string, string> CustomAttributes { get; init; } = new();
}
