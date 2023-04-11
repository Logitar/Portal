using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.v2.Core.Senders.Queries;

internal record GetSenders(ProviderType? Provider, string? Realm, string? Search,
  SenderSort? Sort, bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<Sender>>;
