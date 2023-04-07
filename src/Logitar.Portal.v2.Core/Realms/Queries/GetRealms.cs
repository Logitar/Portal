using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.v2.Core.Realms.Queries;

internal record GetRealms(string? Search, RealmSort? Sort, bool IsDescending,
  int? Skip, int? Limit) : IRequest<PagedList<Realm>>;
