using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateUniqueKeyChangedEvent(UniqueKeyUnit UniqueKey) : DomainEvent, INotification;
