﻿using Logitar.EventSourcing;
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

  public async Task SaveAsync(DictionaryAggregate dictionary, ActorId actorId, CancellationToken cancellationToken)
  {
    bool hasLocaleChanged = false;
    foreach (DomainEvent change in dictionary.Changes)
    {
      if (change is DictionaryCreatedEvent || change is DictionaryLocaleChangedEvent)
      {
        hasLocaleChanged = true;
      }
    }

    if (hasLocaleChanged)
    {
      DictionaryAggregate? other = await _dictionaryRepository.LoadAsync(dictionary.TenantId, dictionary.Locale, cancellationToken);
      if (other?.Equals(dictionary) == false)
      {
        throw new DictionaryAlreadyExistsException(dictionary.TenantId, dictionary.Locale);
      }
    }

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);
  }
}
