using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Queries
{
  internal class GetDictionaryQueryHandler : IRequestHandler<GetDictionaryQuery, DictionaryModel?>
  {
    private readonly IDictionaryQuerier _dictionaryQuerier;

    public GetDictionaryQueryHandler(IDictionaryQuerier dictionaryQuerier)
    {
      _dictionaryQuerier = dictionaryQuerier;
    }

    public async Task<DictionaryModel?> Handle(GetDictionaryQuery request, CancellationToken cancellationToken)
    {
      return await _dictionaryQuerier.GetAsync(request.Id, cancellationToken);
    }
  }
}
