using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Sessions;
using MediatR;

namespace Logitar.Portal.Application.Sessions.Queries;

internal class SearchSessionsQueryHandler : IRequestHandler<SearchSessionsQuery, SearchResults<Session>>
{
  private readonly ISessionQuerier _sessionQuerier;

  public SearchSessionsQueryHandler(ISessionQuerier sessionQuerier)
  {
    _sessionQuerier = sessionQuerier;
  }

  public async Task<SearchResults<Session>> Handle(SearchSessionsQuery query, CancellationToken cancellationToken)
  {
    return await _sessionQuerier.SearchAsync(query.Realm, query.Payload, cancellationToken);
  }
}
