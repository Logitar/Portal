using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Realms.Events;

public record UrlChanged : DomainEvent, INotification
{
  public Uri? Url { get; init; }
}
