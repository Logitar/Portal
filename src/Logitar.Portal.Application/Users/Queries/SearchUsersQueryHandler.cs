using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries;

internal class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, SearchResults<User>>
{
  private readonly IUserQuerier _userQuerier;

  public SearchUsersQueryHandler(IUserQuerier userQuerier)
  {
    _userQuerier = userQuerier;
  }

  public async Task<SearchResults<User>> Handle(SearchUsersQuery query, CancellationToken cancellationToken)
  {
    return await _userQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
