using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Queries;

internal class GetDictionaryHandler : IRequestHandler<GetDictionary, Dictionary?>
{
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public GetDictionaryHandler(IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<Dictionary?> Handle(GetDictionary request, CancellationToken cancellationToken)
  {
    List<Dictionary> dictionaries = new(capacity: 1);

    if (request.Id.HasValue)
    {
      dictionaries.AddIfNotNull(await _dictionaryQuerier.GetAsync(request.Id.Value, cancellationToken));
    }

    if (dictionaries.Count > 1)
    {
      throw new TooManyResultsException();
    }

    return dictionaries.SingleOrDefault();
  }
}
