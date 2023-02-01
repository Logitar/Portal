using Logitar.Portal.Contracts.Accounts.Payloads;
using Logitar.Portal.Contracts.Sessions.Models;
using MediatR;

namespace Logitar.Portal.Application.Accounts.Commands
{
  internal record SignInCommand(SignInPayload Payload, string? Realm) : IRequest<SessionModel>;
}
