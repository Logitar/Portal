﻿using Logitar.Portal.Contracts.Messages;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Messages.Queries;

internal class SearchMessagesQueryHandler : IRequestHandler<SearchMessagesQuery, SearchResults<Message>>
{
  private readonly IMessageQuerier _messageQuerier;

  public SearchMessagesQueryHandler(IMessageQuerier messageQuerier)
  {
    _messageQuerier = messageQuerier;
  }

  public async Task<SearchResults<Message>> Handle(SearchMessagesQuery query, CancellationToken cancellationToken)
  {
    return await _messageQuerier.SearchAsync(query.Realm, query.Payload, cancellationToken);
  }
}
