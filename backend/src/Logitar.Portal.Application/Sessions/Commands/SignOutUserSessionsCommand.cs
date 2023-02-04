using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands
{
  internal record SignOutUserSessionsCommand(string UserId) : IRequest<IEnumerable<SessionModel>>;
}
