using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries;

internal class SearchRealmsQueryHandler : IRequestHandler<SearchRealmsQuery, SearchResults<RealmModel>>
{
  private readonly IRealmQuerier _realmQuerier;

  public SearchRealmsQueryHandler(IRealmQuerier realmQuerier)
  {
    _realmQuerier = realmQuerier;
  }

  public async Task<SearchResults<RealmModel>> Handle(SearchRealmsQuery query, CancellationToken cancellationToken)
  {
    return await _realmQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
