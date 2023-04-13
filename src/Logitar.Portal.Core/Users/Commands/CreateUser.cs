using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal record CreateUser(CreateUserInput Input) : IRequest<User>;
