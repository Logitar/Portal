using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateCreatedEvent(TenantId? TenantId, UniqueKeyUnit UniqueKey, SubjectUnit Subject, ContentUnit Content) : DomainEvent, INotification;
