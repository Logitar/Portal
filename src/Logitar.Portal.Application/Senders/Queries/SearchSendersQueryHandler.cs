using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Queries;

internal class SearchSendersQueryHandler : IRequestHandler<SearchSendersQuery, SearchResults<Sender>>
{
  private readonly ISenderQuerier _senderQuerier;

  public SearchSendersQueryHandler(ISenderQuerier senderQuerier)
  {
    _senderQuerier = senderQuerier;
  }

  public async Task<SearchResults<Sender>> Handle(SearchSendersQuery query, CancellationToken cancellationToken)
  {
    return await _senderQuerier.SearchAsync(query.Payload, cancellationToken);
  }
}
