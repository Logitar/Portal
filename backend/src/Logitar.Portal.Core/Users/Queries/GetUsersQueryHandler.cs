using Logitar.Portal.Core.Models;
using Logitar.Portal.Core.Users.Models;
using MediatR;

namespace Logitar.Portal.Core.Users.Queries
{
  internal class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, ListModel<UserModel>>
  {
    private readonly IUserQuerier _userQuerier;

    public GetUsersQueryHandler(IUserQuerier userQuerier)
    {
      _userQuerier = userQuerier;
    }

    public async Task<ListModel<UserModel>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
      return await _userQuerier.GetPagedAsync(request.IsConfirmed, request.IsDisabled, request.Realm, request.Search,
        request.Sort, request.IsDescending,
        request.Index, request.Count,
        cancellationToken);
    }
  }
}
