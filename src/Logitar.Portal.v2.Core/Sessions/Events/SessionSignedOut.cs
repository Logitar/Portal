using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.v2.Core.Sessions.Events;

public record SessionSignedOut : DomainEvent, INotification;
