using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands
{
  internal record EnableUserCommand(string Id) : IRequest<UserModel>;
}
