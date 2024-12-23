using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Search;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries;

internal record SearchSendersQuery(SearchSendersPayload Payload) : Activity, IRequest<SearchResults<SenderModel>>;

internal class SearchSendersQueryHandler : IRequestHandler<SearchSendersQuery, SearchResults<SenderModel>>
{
  private readonly ISenderQuerier _senderQuerier;

  public SearchSendersQueryHandler(ISenderQuerier senderQuerier)
  {
    _senderQuerier = senderQuerier;
  }

  public async Task<SearchResults<SenderModel>> Handle(SearchSendersQuery query, CancellationToken cancellationToken)
  {
    return await _senderQuerier.SearchAsync(query.Realm, query.Payload, cancellationToken);
  }
}
