using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries
{
  internal record GetSendersQuery(ProviderType? Provider, string? Realm, string? Search,
    SenderSort? Sort, bool IsDescending, int? Index, int? Count) : IRequest<ListModel<SenderModel>>;
}
