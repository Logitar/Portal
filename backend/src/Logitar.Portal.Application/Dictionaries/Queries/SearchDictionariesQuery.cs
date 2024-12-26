using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Contracts.Search;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Queries;

internal record SearchDictionariesQuery(SearchDictionariesPayload Payload) : Activity, IRequest<SearchResults<DictionaryModel>>;

internal class SearchDictionariesQueryHandler : IRequestHandler<SearchDictionariesQuery, SearchResults<DictionaryModel>>
{
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public SearchDictionariesQueryHandler(IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<SearchResults<DictionaryModel>> Handle(SearchDictionariesQuery query, CancellationToken cancellationToken)
  {
    return await _dictionaryQuerier.SearchAsync(query.Realm, query.Payload, cancellationToken);
  }
}
