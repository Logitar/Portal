using Logitar.Portal.Contracts.Tokens;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands
{
  internal record CreateTokenCommand(CreateTokenPayload Payload) : IRequest<TokenModel>;
}
