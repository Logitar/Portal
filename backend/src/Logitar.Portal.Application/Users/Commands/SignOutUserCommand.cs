using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

public record SignOutUserCommand(Guid Id) : Activity, IRequest<UserModel?>;
