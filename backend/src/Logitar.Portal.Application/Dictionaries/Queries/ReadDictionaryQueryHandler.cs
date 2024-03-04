using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Queries;

internal class ReadDictionaryQueryHandler : IRequestHandler<ReadDictionaryQuery, Dictionary?>
{
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public ReadDictionaryQueryHandler(IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<Dictionary?> Handle(ReadDictionaryQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, Dictionary> dictionaries = new(capacity: 2);

    if (query.Id.HasValue)
    {
      Dictionary? dictionary = await _dictionaryQuerier.ReadAsync(query.Realm, query.Id.Value, cancellationToken);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Locale))
    {
      Dictionary? dictionary = await _dictionaryQuerier.ReadAsync(query.Realm, query.Locale, cancellationToken);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (dictionaries.Count > 1)
    {
      throw new TooManyResultsException<Dictionary>(expectedCount: 1, actualCount: dictionaries.Count);
    }

    return dictionaries.Values.SingleOrDefault();
  }
}
