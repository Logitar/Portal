using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Messages.Queries;

internal record SearchMessagesQuery(SearchMessagesPayload Payload) : Activity, IRequest<SearchResults<MessageModel>>;

internal class SearchMessagesQueryHandler : IRequestHandler<SearchMessagesQuery, SearchResults<MessageModel>>
{
  private readonly IMessageQuerier _messageQuerier;

  public SearchMessagesQueryHandler(IMessageQuerier messageQuerier)
  {
    _messageQuerier = messageQuerier;
  }

  public async Task<SearchResults<MessageModel>> Handle(SearchMessagesQuery query, CancellationToken cancellationToken)
  {
    return await _messageQuerier.SearchAsync(query.Realm, query.Payload, cancellationToken);
  }
}
