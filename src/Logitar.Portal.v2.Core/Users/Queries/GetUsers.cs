using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Users;
using MediatR;

namespace Logitar.Portal.v2.Core.Users.Queries;

internal record GetUsers(bool? IsConfirmed, bool? IsDisabled, string? Realm, string? Search,
  UserSort? Sort, bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<User>>;
