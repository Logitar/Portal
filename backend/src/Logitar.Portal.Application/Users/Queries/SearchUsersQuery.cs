using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

internal record SearchUsersQuery(SearchUsersPayload Payload) : Activity, IRequest<SearchResults<UserModel>>;

internal class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, SearchResults<UserModel>>
{
  private readonly IUserQuerier _sessionQuerier;

  public SearchUsersQueryHandler(IUserQuerier sessionQuerier)
  {
    _sessionQuerier = sessionQuerier;
  }

  public async Task<SearchResults<UserModel>> Handle(SearchUsersQuery query, CancellationToken cancellationToken)
  {
    return await _sessionQuerier.SearchAsync(query.Realm, query.Payload, cancellationToken);
  }
}
