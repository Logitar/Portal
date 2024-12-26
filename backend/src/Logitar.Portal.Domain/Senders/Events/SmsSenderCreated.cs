using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SmsSenderCreated(TenantId? TenantId, PhoneUnit Phone, SenderProvider Provider) : DomainEvent, INotification;
