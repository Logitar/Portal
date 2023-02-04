using MediatR;

namespace Logitar.Portal.Application.Senders.Commands
{
  internal record DeleteSenderCommand(string Id) : IRequest;
}
