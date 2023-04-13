using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal record DisableUser(Guid Id) : IRequest<User>;
