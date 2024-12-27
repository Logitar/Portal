using Logitar.EventSourcing;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Dictionaries.Events;

namespace Logitar.Portal.Application.Dictionaries;

internal class DictionaryManager : IDictionaryManager
{
  private readonly IDictionaryRepository _dictionaryRepository;

  public DictionaryManager(IDictionaryRepository dictionaryRepository)
  {
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task SaveAsync(Dictionary dictionary, ActorId actorId, CancellationToken cancellationToken)
  {
    bool hasLocaleChanged = false;
    foreach (IEvent change in dictionary.Changes)
    {
      if (change is DictionaryCreated || change is DictionaryLocaleChanged)
      {
        hasLocaleChanged = true;
      }
    }

    if (hasLocaleChanged)
    {
      Dictionary? conflict = await _dictionaryRepository.LoadAsync(dictionary.TenantId, dictionary.Locale, cancellationToken);
      if (conflict != null && !conflict.Equals(dictionary))
      {
        throw new DictionaryAlreadyExistsException(dictionary, conflict.Id);
      }
    }

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);
  }
}
