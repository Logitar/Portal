using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Queries;

internal record SearchSessionsQuery(SearchSessionsPayload Payload) : IRequest<SearchResults<Session>>;
