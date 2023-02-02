using MediatR;

namespace Logitar.Portal.Application.ApiKeys.Commands
{
  internal record DeleteApiKeyCommand(string Id) : IRequest;
}
