using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands
{
  internal record SignOutCommand(string Id) : IRequest;
}
