using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Core.Realms.Queries;

internal record GetRealms(string? Search, RealmSort? Sort, bool IsDescending,
  int? Skip, int? Limit) : IRequest<PagedList<Realm>>;
