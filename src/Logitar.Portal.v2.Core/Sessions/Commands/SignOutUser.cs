using Logitar.Portal.v2.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.v2.Core.Sessions.Commands;

internal record SignOutUser(Guid Id) : IRequest<IEnumerable<Session>>;
