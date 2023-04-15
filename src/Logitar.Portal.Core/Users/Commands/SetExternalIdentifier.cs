using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal record SetExternalIdentifier(Guid Id, string Key, string? Value) : IRequest<User>;
