using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal record EnableUser(Guid Id) : IRequest<User>;
