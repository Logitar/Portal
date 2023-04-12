using Logitar.Portal.v2.Core.Senders;
using MediatR;

namespace Logitar.Portal.v2.Core.Messages;

public record SendEmail(MessageAggregate Message, SenderAggregate Sender) : INotification;
