using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Queries;

internal record ReadSessionQuery(Guid Id) : Activity, IRequest<Session?>;
