using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts;
using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Queries;

internal record ReadDictionaryQuery(Guid? Id, string? Locale) : Activity, IRequest<DictionaryModel>;

internal class ReadDictionaryQueryHandler : IRequestHandler<ReadDictionaryQuery, DictionaryModel?>
{
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public ReadDictionaryQueryHandler(IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<DictionaryModel?> Handle(ReadDictionaryQuery query, CancellationToken cancellationToken)
  {
    Dictionary<Guid, DictionaryModel> dictionaries = new(capacity: 2);

    if (query.Id.HasValue)
    {
      DictionaryModel? dictionary = await _dictionaryQuerier.ReadAsync(query.Realm, query.Id.Value, cancellationToken);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (!string.IsNullOrWhiteSpace(query.Locale))
    {
      DictionaryModel? dictionary = await _dictionaryQuerier.ReadAsync(query.Realm, query.Locale, cancellationToken);
      if (dictionary != null)
      {
        dictionaries[dictionary.Id] = dictionary;
      }
    }

    if (dictionaries.Count > 1)
    {
      throw new TooManyResultsException<DictionaryModel>(expectedCount: 1, actualCount: dictionaries.Count);
    }

    return dictionaries.Values.SingleOrDefault();
  }
}
