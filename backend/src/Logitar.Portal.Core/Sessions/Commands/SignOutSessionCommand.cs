using Logitar.Portal.Core.Sessions.Models;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands
{
  internal class SignOutSessionCommand : IRequest<SessionModel>
  {
    public SignOutSessionCommand(string id)
    {
      Id = id;
    }

    public string Id { get; }
  }
}
