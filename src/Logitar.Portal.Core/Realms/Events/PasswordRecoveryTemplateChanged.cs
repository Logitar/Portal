using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Realms.Events;

public record PasswordRecoveryTemplateChanged : DomainEvent, INotification
{
  public AggregateId? TemplateId { get; init; }
}
