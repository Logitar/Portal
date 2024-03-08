using Logitar.Portal.Contracts.Tokens;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands;

internal record ValidateTokenCommand(ValidateTokenPayload Payload) : Activity,IRequest<ValidatedToken>;
