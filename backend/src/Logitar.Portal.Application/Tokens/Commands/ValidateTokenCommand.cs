using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Tokens;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands;

public record ValidateTokenCommand(ValidateTokenPayload Payload) : Activity, IRequest<ValidatedToken>;
