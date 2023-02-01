using Logitar.Portal.Contracts.Accounts.Payloads;
using Logitar.Portal.Contracts.Users.Models;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal record ChangePasswordCommand(ChangePasswordPayload Payload) : IRequest<UserModel>;
}
