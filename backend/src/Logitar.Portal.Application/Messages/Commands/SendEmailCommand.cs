using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

public record SendEmailCommand(MessageAggregate Message, SenderAggregate Sender) : INotification;
