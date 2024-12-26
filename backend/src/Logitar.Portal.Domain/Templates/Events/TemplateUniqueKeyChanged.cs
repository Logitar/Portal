using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateUniqueKeyChanged(UniqueKeyUnit UniqueKey) : DomainEvent, INotification;
