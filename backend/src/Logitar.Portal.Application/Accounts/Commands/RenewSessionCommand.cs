using Logitar.Portal.Contracts.Accounts;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal record RenewSessionCommand(RenewSessionPayload Payload, string? Realm) : IRequest<SessionModel>;
}
