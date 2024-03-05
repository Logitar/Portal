using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal class SearchRolesQueryHandler : IRequestHandler<SearchRolesQuery, SearchResults<Role>>
{
  private readonly IRoleQuerier _roleQuerier;

  public SearchRolesQueryHandler(IRoleQuerier roleQuerier)
  {
    _roleQuerier = roleQuerier;
  }

  public async Task<SearchResults<Role>> Handle(SearchRolesQuery query, CancellationToken cancellationToken)
  {
    return await _roleQuerier.SearchAsync(query.Realm, query.Payload, cancellationToken);
  }
}
