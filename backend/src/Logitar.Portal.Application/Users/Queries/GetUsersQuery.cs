using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Application.Users.Queries
{
  internal record GetUsersQuery(bool? IsConfirmed, bool? IsDisabled, string? Realm, string? Search,
    UserSort? Sort, bool IsDecending, int? Index, int? Count) : IRequest<ListModel<UserModel>>;
}
