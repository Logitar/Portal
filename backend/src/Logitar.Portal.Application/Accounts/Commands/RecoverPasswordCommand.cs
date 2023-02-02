using Logitar.Portal.Contracts.Accounts;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal record RecoverPasswordCommand(RecoverPasswordPayload Payload, string Realm) : IRequest;
}
