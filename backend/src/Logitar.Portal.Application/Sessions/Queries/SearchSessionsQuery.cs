using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Queries;

internal record SearchSessionsQuery(SearchSessionsPayload Payload) : Activity, IRequest<SearchResults<Session>>;
