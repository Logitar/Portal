using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.Queries
{
  internal record GetRealmsQuery(string? Search, RealmSort? Sort, bool IsDecending, int? Index, int? Count)
    : IRequest<ListModel<RealmModel>>;
}
