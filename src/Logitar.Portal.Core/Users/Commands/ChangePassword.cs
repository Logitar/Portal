using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Commands;

internal record ChangePassword(Guid Id, ChangePasswordInput Input) : IRequest<User>;
