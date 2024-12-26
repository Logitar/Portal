using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateCreated(TenantId? TenantId, UniqueKey UniqueKey, Subject Subject, Content Content) : DomainEvent, INotification;
