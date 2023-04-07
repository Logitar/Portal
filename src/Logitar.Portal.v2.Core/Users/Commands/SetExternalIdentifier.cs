using Logitar.Portal.v2.Contracts.Users;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Commands;

internal record SetExternalIdentifier(Guid Id, string Key, string? Value) : IRequest<User>;
