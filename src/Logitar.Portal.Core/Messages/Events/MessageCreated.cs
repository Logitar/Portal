using Logitar.EventSourcing;
using Logitar.Portal.Core.Messages.Summaries;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Core.Messages.Events;

public record MessageCreated : DomainEvent, INotification
{
  public bool IsDemo { get; init; }

  public string Subject { get; init; } = string.Empty;
  public string Body { get; init; } = string.Empty;

  public IEnumerable<Recipient> Recipients { get; init; } = Enumerable.Empty<Recipient>();

  public RealmSummary? Realm { get; init; }
  public SenderSummary Sender { get; init; } = new();
  public TemplateSummary Template { get; init; } = new();

  public bool IgnoreUserLocale { get; set; }
  public CultureInfo? Locale { get; set; }

  public Dictionary<string, string> Variables { get; set; } = new();
}
