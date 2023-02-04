using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal record ChangePasswordCommand(ChangePasswordPayload Payload) : IRequest<UserModel>;
}
