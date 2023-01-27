using Logitar.Portal.Core.Accounts.Payloads;
using Logitar.Portal.Core.Users.Models;
using MediatR;

namespace Logitar.Portal.Core.Accounts.Commands
{
  internal class ChangePasswordCommand : IRequest<UserModel>
  {
    public ChangePasswordCommand(ChangePasswordPayload payload)
    {
      Payload = payload;
    }

    public ChangePasswordPayload Payload { get; }
  }
}
