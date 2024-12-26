using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Dictionaries.Validators;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal record UpdateDictionaryCommand(Guid Id, UpdateDictionaryPayload Payload) : Activity, IRequest<DictionaryModel?>;

internal class UpdateDictionaryCommandHandler : IRequestHandler<UpdateDictionaryCommand, DictionaryModel?>
{
  private readonly IDictionaryManager _dictionaryManager;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public UpdateDictionaryCommandHandler(IDictionaryManager dictionaryManager, IDictionaryRepository dictionaryRepository, IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryManager = dictionaryManager;
    _dictionaryRepository = dictionaryRepository;
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<DictionaryModel?> Handle(UpdateDictionaryCommand command, CancellationToken cancellationToken)
  {
    UpdateDictionaryPayload payload = command.Payload;
    new UpdateDictionaryValidator().ValidateAndThrow(payload);

    Dictionary? dictionary = await _dictionaryRepository.LoadAsync(command.Id, cancellationToken);
    if (dictionary == null || dictionary.TenantId != command.TenantId)
    {
      return null;
    }

    ActorId actorId = command.ActorId;

    LocaleUnit? locale = LocaleUnit.TryCreate(payload.Locale);
    if (locale != null)
    {
      dictionary.SetLocale(locale, actorId);
    }

    foreach (DictionaryEntryModification entry in payload.Entries)
    {
      if (entry.Value == null)
      {
        dictionary.RemoveEntry(entry.Key);
      }
      else
      {
        dictionary.SetEntry(entry.Key, entry.Value);
      }
    }
    dictionary.Update(actorId);

    await _dictionaryManager.SaveAsync(dictionary, actorId, cancellationToken);

    return await _dictionaryQuerier.ReadAsync(command.Realm, dictionary, cancellationToken);
  }
}
