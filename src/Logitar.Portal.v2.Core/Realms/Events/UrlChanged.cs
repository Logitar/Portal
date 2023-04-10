using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Events;

public record UrlChanged : DomainEvent, INotification
{
  public Uri? Url { get; init; }
}
