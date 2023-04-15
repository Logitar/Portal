using MediatR;

namespace Logitar.Portal.Core.Users.Events;

public record UserUpdated : UserSaved, INotification;
