using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record RemoveIdentifierCommand(Guid Id, string Key) : IRequest<User?>;
