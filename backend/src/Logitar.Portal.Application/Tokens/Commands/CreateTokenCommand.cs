using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Tokens;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands;

internal record CreateTokenCommand(CreateTokenPayload Payload) : Activity, IRequest<CreatedToken>;
