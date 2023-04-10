using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Events;

public record UserSignedIn : DomainEvent, INotification;
