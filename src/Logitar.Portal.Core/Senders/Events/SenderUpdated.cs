using MediatR;

namespace Logitar.Portal.Core.Senders.Events;

public record SenderUpdated : SenderSaved, INotification;
