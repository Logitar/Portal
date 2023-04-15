using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Users;
using MediatR;

namespace Logitar.Portal.Core.Users.Queries;

internal record GetUsers(bool? IsConfirmed, bool? IsDisabled, string? Realm, string? Search,
  UserSort? Sort, bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<User>>;
