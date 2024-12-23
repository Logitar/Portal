﻿using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Roles;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Roles.Queries;

internal record SearchRolesQuery(SearchRolesPayload Payload) : Activity, IRequest<SearchResults<RoleModel>>;

internal class SearchRolesQueryHandler : IRequestHandler<SearchRolesQuery, SearchResults<RoleModel>>
{
  private readonly IRoleQuerier _roleQuerier;

  public SearchRolesQueryHandler(IRoleQuerier roleQuerier)
  {
    _roleQuerier = roleQuerier;
  }

  public async Task<SearchResults<RoleModel>> Handle(SearchRolesQuery query, CancellationToken cancellationToken)
  {
    return await _roleQuerier.SearchAsync(query.Realm, query.Payload, cancellationToken);
  }
}
