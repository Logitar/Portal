using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Commands;

internal record RemoveUserIdentifierCommand(Guid Id, string Key) : Activity, IRequest<UserModel?>;
