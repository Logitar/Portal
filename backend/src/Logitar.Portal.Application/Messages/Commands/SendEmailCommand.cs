using Logitar.EventSourcing;
using Logitar.Portal.Domain.Messages;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands;

public record SendEmailCommand(ActorId ActorId, MessageAggregate Message, Sender Sender) : IRequest;
