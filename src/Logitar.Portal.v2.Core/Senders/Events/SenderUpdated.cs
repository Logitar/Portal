using MediatR;

namespace Logitar.Portal.v2.Core.Senders.Events;

public record SenderUpdated : SenderSaved, INotification;
