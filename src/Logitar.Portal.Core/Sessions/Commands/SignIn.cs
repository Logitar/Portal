using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands;

internal record SignIn(SignInInput Input, string? Realm = null) : IRequest<Session>;
