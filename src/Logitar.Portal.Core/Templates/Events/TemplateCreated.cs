using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Templates.Events;

public record TemplateCreated : TemplateSaved, INotification
{
  public AggregateId RealmId { get; init; }

  public string UniqueName { get; init; } = string.Empty;
}
