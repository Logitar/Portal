using Logitar.Portal.v2.Contracts;
using Logitar.Portal.v2.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.v2.Core.Dictionaries.Queries
{
  internal class GetDictionariesHandler : IRequestHandler<GetDictionaries, PagedList<Dictionary>>
  {
    private readonly IDictionaryQuerier _dictionaryQuerier;

    public GetDictionariesHandler(IDictionaryQuerier dictionaryQuerier)
    {
      _dictionaryQuerier = dictionaryQuerier;
    }

    public async Task<PagedList<Dictionary>> Handle(GetDictionaries request, CancellationToken cancellationToken)
    {
      return await _dictionaryQuerier.GetAsync(request.Locale, request.Realm, request.Sort,
        request.IsDescending, request.Skip, request.Limit, cancellationToken);
    }
  }
}
