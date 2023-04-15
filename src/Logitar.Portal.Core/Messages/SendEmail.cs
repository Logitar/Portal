using Logitar.Portal.Core.Senders;
using MediatR;

namespace Logitar.Portal.Core.Messages;

public record SendEmail(MessageAggregate Message, SenderAggregate Sender) : INotification;
