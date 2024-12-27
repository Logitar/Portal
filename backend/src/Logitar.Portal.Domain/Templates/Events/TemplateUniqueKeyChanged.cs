using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateUniqueKeyChanged(Identifier UniqueKey) : DomainEvent, INotification;
