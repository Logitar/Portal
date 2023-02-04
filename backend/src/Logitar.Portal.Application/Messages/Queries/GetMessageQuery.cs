using Logitar.Portal.Contracts.Messages;
using MediatR;

namespace Logitar.Portal.Application.Messages.Queries
{
  internal record GetMessageQuery(string Id) : IRequest<MessageModel?>;
}
