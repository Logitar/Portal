using MediatR;

namespace Logitar.Portal.v2.Core.Users.Events;

public record UserUpdated : UserSaved, INotification;
