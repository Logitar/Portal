using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Dictionaries.Validators;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal class ReplaceDictionaryCommandHandler : IRequestHandler<ReplaceDictionaryCommand, DictionaryModel?>
{
  private readonly IDictionaryManager _dictionaryManager;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public ReplaceDictionaryCommandHandler(IDictionaryManager dictionaryManager, IDictionaryRepository dictionaryRepository, IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryManager = dictionaryManager;
    _dictionaryRepository = dictionaryRepository;
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<DictionaryModel?> Handle(ReplaceDictionaryCommand command, CancellationToken cancellationToken)
  {
    ReplaceDictionaryPayload payload = command.Payload;
    new ReplaceDictionaryValidator().ValidateAndThrow(payload);

    Dictionary? dictionary = await _dictionaryRepository.LoadAsync(command.Id, cancellationToken);
    if (dictionary == null || dictionary.TenantId != command.TenantId)
    {
      return null;
    }
    Dictionary? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _dictionaryRepository.LoadAsync(dictionary.Id, command.Version.Value, cancellationToken);
    }

    ActorId actorId = command.ActorId;

    LocaleUnit locale = new(payload.Locale);
    if (reference == null || locale != reference.Locale)
    {
      dictionary.SetLocale(locale, actorId);
    }

    ReplaceDictionaryEntries(payload, dictionary, reference);
    dictionary.Update(actorId);

    await _dictionaryManager.SaveAsync(dictionary, actorId, cancellationToken);

    return await _dictionaryQuerier.ReadAsync(command.Realm, dictionary, cancellationToken);
  }

  private static void ReplaceDictionaryEntries(ReplaceDictionaryPayload payload, Dictionary dictionary, Dictionary? reference)
  {
    HashSet<string> payloadKeys = new(capacity: payload.Entries.Count);

    IEnumerable<string> referenceKeys;
    if (reference == null)
    {
      referenceKeys = dictionary.Entries.Keys;

      foreach (DictionaryEntry customAttribute in payload.Entries)
      {
        payloadKeys.Add(customAttribute.Key.Trim());
        dictionary.SetEntry(customAttribute.Key, customAttribute.Value);
      }
    }
    else
    {
      referenceKeys = reference.Entries.Keys;

      foreach (DictionaryEntry customAttribute in payload.Entries)
      {
        string key = customAttribute.Key.Trim();
        payloadKeys.Add(key);

        string value = customAttribute.Value.Trim();
        if (!reference.Entries.TryGetValue(key, out string? existingValue) || existingValue != value)
        {
          dictionary.SetEntry(key, value);
        }
      }
    }

    foreach (string key in referenceKeys)
    {
      if (!payloadKeys.Contains(key))
      {
        dictionary.RemoveEntry(key);
      }
    }
  }
}
