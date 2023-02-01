using Logitar.Portal.Contracts.Users.Models;
using Logitar.Portal.Contracts.Users.Payloads;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands
{
  internal record UpdateUserCommand(string Id, UpdateUserPayload Payload) : IRequest<UserModel>;
}
