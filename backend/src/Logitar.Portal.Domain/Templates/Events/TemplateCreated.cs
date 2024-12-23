using Logitar.EventSourcing;
using Logitar.Identity.Core;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateCreated(Identifier UniqueKey, Subject Subject, Content Content) : DomainEvent, INotification;
