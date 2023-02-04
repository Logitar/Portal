using Logitar.Portal.Contracts.Senders;
using MediatR;
using System.Globalization;

namespace Logitar.Portal.Domain.Messages.Events
{
  public record MessageCreatedEvent : DomainEvent, INotification
  {
    public string Subject { get; init; } = string.Empty;
    public string Body { get; init; } = string.Empty;
    public IEnumerable<Recipient> Recipients { get; init; } = Enumerable.Empty<Recipient>();

    public AggregateId SenderId { get; init; }
    public bool SenderIsDefault { get; init; }
    public string SenderAddress { get; init; } = string.Empty;
    public string? SenderDisplayName { get; init; }
    public ProviderType SenderProvider { get; init; }

    public AggregateId TemplateId { get; init; }
    public string TemplateKey { get; init; } = string.Empty;
    public string? TemplateDisplayName { get; init; }
    public string TemplateContentType { get; init; } = string.Empty;

    public AggregateId? RealmId { get; init; }
    public string? RealmAlias { get; init; }
    public string? RealmDisplayName { get; init; }

    public bool IgnoreUserLocale { get; init; }
    public CultureInfo? Locale { get; init; }

    public Dictionary<string, string?>? Variables { get; init; }

    public bool IsDemo { get; init; }
  }
}
