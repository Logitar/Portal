using Logitar.Portal.Application.Users;
using Logitar.Portal.Application.Users.Queries;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Users;
using MediatR;

internal class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, SearchResults<User>>
{
  private readonly IUserQuerier _sessionQuerier;

  public SearchUsersQueryHandler(IUserQuerier sessionQuerier)
  {
    _sessionQuerier = sessionQuerier;
  }

  public async Task<SearchResults<User>> Handle(SearchUsersQuery query, CancellationToken cancellationToken)
  {
    return await _sessionQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
