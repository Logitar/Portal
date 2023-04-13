using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands;

internal record SignIn(SignInInput Input) : IRequest<Session>;
