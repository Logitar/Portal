using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Core.Senders.Queries;

internal record GetSenders(ProviderType? Provider, string? Realm, string? Search,
  SenderSort? Sort, bool IsDescending, int? Skip, int? Limit) : IRequest<PagedList<Sender>>;
