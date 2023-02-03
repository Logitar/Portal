using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands
{
  internal record UpdateSenderCommand(string Id, UpdateSenderPayload Payload) : IRequest<SenderModel>;
}
