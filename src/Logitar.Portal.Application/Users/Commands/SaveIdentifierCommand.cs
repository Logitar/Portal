using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record SaveIdentifierCommand(Guid Id, SaveIdentifierPayload Payload) : IRequest<User?>;
