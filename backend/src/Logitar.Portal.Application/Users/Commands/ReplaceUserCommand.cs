using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record ReplaceUserCommand(Guid Id, ReplaceUserPayload Payload, long? Version) : IRequest<User?>;
