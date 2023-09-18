using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands;

internal record CreateTokenCommand(CreateTokenPayload Payload, RealmAggregate? Realm = null) : IRequest<CreatedToken>;
