using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands;

internal record SignOutUser(Guid Id) : IRequest<IEnumerable<Session>>;
