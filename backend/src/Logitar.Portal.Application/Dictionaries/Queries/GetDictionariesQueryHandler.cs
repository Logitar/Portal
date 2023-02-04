using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Queries
{
  internal class GetDictionariesQueryHandler : IRequestHandler<GetDictionariesQuery, ListModel<DictionaryModel>>
  {
    private readonly IDictionaryQuerier _dictionaryQuerier;

    public GetDictionariesQueryHandler(IDictionaryQuerier dictionaryQuerier)
    {
      _dictionaryQuerier = dictionaryQuerier;
    }

    public async Task<ListModel<DictionaryModel>> Handle(GetDictionariesQuery request, CancellationToken cancellationToken)
    {
      return await _dictionaryQuerier.GetPagedAsync(request.Locale, request.Realm,
        request.Sort, request.IsDescending,
        request.Index, request.Count,
        cancellationToken);
    }
  }
}
