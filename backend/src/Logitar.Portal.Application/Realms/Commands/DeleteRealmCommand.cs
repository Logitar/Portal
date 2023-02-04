using MediatR;

namespace Logitar.Portal.Application.Realms.Commands
{
  internal record DeleteRealmCommand(string Id) : IRequest;
}
