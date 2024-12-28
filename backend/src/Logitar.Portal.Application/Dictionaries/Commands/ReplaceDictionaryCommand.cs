using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Dictionaries.Validators;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal record ReplaceDictionaryCommand(Guid Id, ReplaceDictionaryPayload Payload, long? Version) : Activity, IRequest<DictionaryModel?>;

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

    Locale locale = new(payload.Locale);
    ActorId actorId = command.ActorId;

    DictionaryId dictionaryId = new(command.TenantId, new EntityId(command.Id));
    Dictionary? dictionary = await _dictionaryRepository.LoadAsync(dictionaryId, cancellationToken);
    if (dictionary == null)
    {
      if (command.Version.HasValue)
      {
        return null;
      }

      dictionary = new(locale, actorId, dictionaryId);
    }
    Dictionary? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _dictionaryRepository.LoadAsync(dictionary.Id, command.Version.Value, cancellationToken);
    }

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
    HashSet<Identifier> payloadKeys = new(capacity: payload.Entries.Count);

    IEnumerable<Identifier> referenceKeys;
    if (reference == null)
    {
      referenceKeys = dictionary.Entries.Keys;

      foreach (DictionaryEntry customAttribute in payload.Entries)
      {
        Identifier key = new(customAttribute.Key);
        payloadKeys.Add(key);
        dictionary.SetEntry(key, customAttribute.Value);
      }
    }
    else
    {
      referenceKeys = reference.Entries.Keys;

      foreach (DictionaryEntry customAttribute in payload.Entries)
      {
        Identifier key = new(customAttribute.Key);
        payloadKeys.Add(key);

        string value = customAttribute.Value.Trim();
        if (!reference.Entries.TryGetValue(key, out string? existingValue) || existingValue != value)
        {
          dictionary.SetEntry(key, value);
        }
      }
    }

    foreach (Identifier key in referenceKeys)
    {
      if (!payloadKeys.Contains(key))
      {
        dictionary.RemoveEntry(key);
      }
    }
  }
}
