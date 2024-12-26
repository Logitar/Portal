using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using MediatR;

namespace Logitar.Portal.Domain.Dictionaries.Events;

public record DictionaryCreated(TenantId? TenantId, LocaleUnit Locale) : DomainEvent, INotification;
