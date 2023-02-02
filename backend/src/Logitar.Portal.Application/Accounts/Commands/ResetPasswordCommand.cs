using Logitar.Portal.Contracts.Accounts;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal record ResetPasswordCommand(ResetPasswordPayload Payload, string Realm) : IRequest;
}
