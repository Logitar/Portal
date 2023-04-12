using Logitar.EventSourcing;

namespace Logitar.Portal.v2.Core.Senders.Events;

public abstract record SenderSaved : DomainEvent
{
  public string EmailAddress { get; init; } = string.Empty;
  public string? DisplayName { get; init; }

  public Dictionary<string, string> Settings { get; init; } = new();
}
