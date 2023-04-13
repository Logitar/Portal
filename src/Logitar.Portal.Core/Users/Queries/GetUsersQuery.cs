using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Queries;

internal class GetUsersQuery : IRequestHandler<GetUsers, PagedList<User>>
{
  private readonly IUserQuerier _userQuerier;

  public GetUsersQuery(IUserQuerier userQuerier)
  {
    _userQuerier = userQuerier;
  }

  public async Task<PagedList<User>> Handle(GetUsers request, CancellationToken cancellationToken)
  {
    return await _userQuerier.GetAsync(request.IsConfirmed, request.IsDisabled, request.Realm, request.Search,
      request.Sort, request.IsDescending, request.Skip, request.Limit, cancellationToken);
  }
}
