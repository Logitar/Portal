using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Dictionaries.Validators;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal record CreateDictionaryCommand(CreateDictionaryPayload Payload) : Activity, IRequest<DictionaryModel>;

internal class CreateDictionaryCommandHandler : IRequestHandler<CreateDictionaryCommand, DictionaryModel>
{
  private readonly IDictionaryManager _dictionaryManager;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly IDictionaryRepository _dictionaryRepository;

  public CreateDictionaryCommandHandler(IDictionaryManager dictionaryManager, IDictionaryQuerier dictionaryQuerier, IDictionaryRepository dictionaryRepository)
  {
    _dictionaryManager = dictionaryManager;
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task<DictionaryModel> Handle(CreateDictionaryCommand command, CancellationToken cancellationToken)
  {
    CreateDictionaryPayload payload = command.Payload;
    new CreateDictionaryValidator().ValidateAndThrow(payload);

    DictionaryId dictionaryId = DictionaryId.NewId(command.TenantId);
    Dictionary? dictionary;
    if (payload.Id.HasValue)
    {
      dictionaryId = new(command.TenantId, new EntityId(payload.Id.Value));
      dictionary = await _dictionaryRepository.LoadAsync(dictionaryId, cancellationToken);
      if (dictionary != null)
      {
        throw new IdAlreadyUsedException(payload.Id.Value, nameof(payload.Id));
      }
    }

    ActorId actorId = command.ActorId;

    Locale locale = new(payload.Locale);
    dictionary = new(locale, actorId, dictionaryId);

    await _dictionaryManager.SaveAsync(dictionary, actorId, cancellationToken);

    return await _dictionaryQuerier.ReadAsync(command.Realm, dictionary, cancellationToken);
  }
}
