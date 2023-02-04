using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Application.Messages.Commands
{
  internal record SendMessageCommand(SendMessagePayload Payload) : IRequest<SentMessagesModel>;
}
