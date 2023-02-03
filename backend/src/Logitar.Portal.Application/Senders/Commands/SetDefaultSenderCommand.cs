using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands
{
  internal record SetDefaultSenderCommand(string Id) : IRequest<SenderModel>;
}
