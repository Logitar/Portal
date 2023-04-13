using Logitar.EventSourcing;
using MediatR;

namespace Logitar.Portal.Core.Users.Events;

public record UserSignedIn : DomainEvent, INotification;
