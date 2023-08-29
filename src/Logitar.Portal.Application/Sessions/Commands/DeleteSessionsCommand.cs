using Logitar.Portal.Domain.Users;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Commands;

internal record DeleteSessionsCommand(UserAggregate User) : INotification;
