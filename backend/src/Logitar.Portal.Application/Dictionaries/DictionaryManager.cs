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
      Dictionary? other = await _dictionaryRepository.LoadAsync(dictionary.TenantId, dictionary.Locale, cancellationToken);
      if (other?.Equals(dictionary) == false)
      {
        throw new DictionaryAlreadyExistsException(dictionary.TenantId, dictionary.Locale);
      }
    }

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);
  }
}
