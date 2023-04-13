using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Events;

public record SessionSignedOut : DomainEvent, INotification;
