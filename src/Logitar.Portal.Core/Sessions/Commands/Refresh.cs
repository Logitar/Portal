using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Core.Sessions.Commands;

internal record Refresh(RefreshInput Input) : IRequest<Session>;
