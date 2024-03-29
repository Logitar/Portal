﻿using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Queries;

internal class SearchDictionariesQueryHandler : IRequestHandler<SearchDictionariesQuery, SearchResults<Dictionary>>
{
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public SearchDictionariesQueryHandler(IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<SearchResults<Dictionary>> Handle(SearchDictionariesQuery query, CancellationToken cancellationToken)
  {
    return await _dictionaryQuerier.SearchAsync(query.Realm, query.Payload, cancellationToken);
  }
}
