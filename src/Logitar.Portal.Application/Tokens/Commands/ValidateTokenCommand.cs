using Logitar.Portal.Contracts.Tokens;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Tokens.Commands;

internal record ValidateTokenCommand(ValidateTokenPayload Payload, RealmAggregate? Realm = null) : IRequest<ValidatedToken>;
