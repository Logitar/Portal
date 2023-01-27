using Logitar.Portal.Core.Sessions.Models;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands
{
  internal class SignOutSessionsCommand : IRequest<IEnumerable<SessionModel>>
  {
    public SignOutSessionsCommand(string userId)
    {
      UserId = userId;
    }

    public string UserId { get; }
  }
}
