using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Domain.Templates.Events;

public record TemplateCreatedEvent : DomainEvent, INotification
{
  public TenantId? TenantId { get; }

  public UniqueKeyUnit UniqueKey { get; }

  public SubjectUnit Subject { get; }
  public ContentUnit Content { get; }

  public TemplateCreatedEvent(ActorId actorId, ContentUnit content, SubjectUnit subject, TenantId? tenantId, UniqueKeyUnit uniqueKey)
  {
    ActorId = actorId;
    Content = content;
    Subject = subject;
    TenantId = tenantId;
    UniqueKey = uniqueKey;
  }
}
