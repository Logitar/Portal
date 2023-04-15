using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal record DeleteUser(Guid Id) : IRequest<User>;
