using MediatR;

namespace Logitar.Portal.Application.Users.Commands
{
  internal record DeleteUserCommand(string Id) : IRequest;
}
