using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Messages.Events;

public record MessageCreatedEvent : DomainEvent, INotification
{
  public MessageCreatedEvent(ActorId actorId)
  {
    ActorId = actorId;
  }

  public string Subject { get; init; } = string.Empty;
  public string Body { get; init; } = string.Empty;

  public IEnumerable<ReadOnlyRecipient> Recipients { get; init; } = Enumerable.Empty<ReadOnlyRecipient>();

  public RealmSummary? Realm { get; init; }
  public SenderSummary Sender { get; init; } = new();
  public TemplateSummary Template { get; init; } = new();

  public bool IgnoreUserLocale { get; init; }
  public ReadOnlyLocale? Locale { get; init; }

  public Dictionary<string, string> Variables { get; init; } = new();

  public bool IsDemo { get; init; }
}
