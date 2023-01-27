using Logitar.Portal.Core.Users.Models;
using Logitar.Portal.Core.Users.Payloads;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands
{
  internal class UpdateUserCommand : IRequest<UserModel>
  {
    public UpdateUserCommand(string id, UpdateUserPayload payload)
    {
      Id = id;
      Payload = payload;
    }

    public string Id { get; }
    public UpdateUserPayload Payload { get; }
  }
}
