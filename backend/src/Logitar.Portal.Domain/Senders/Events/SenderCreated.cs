using Logitar.EventSourcing;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Domain.Senders.Events;

public record SenderCreated(Email Email, SenderProvider Provider) : DomainEvent, INotification;
