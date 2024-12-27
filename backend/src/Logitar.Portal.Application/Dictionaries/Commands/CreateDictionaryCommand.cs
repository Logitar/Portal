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

  public CreateDictionaryCommandHandler(IDictionaryManager dictionaryManager, IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryManager = dictionaryManager;
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<DictionaryModel> Handle(CreateDictionaryCommand command, CancellationToken cancellationToken)
  {
    CreateDictionaryPayload payload = command.Payload;
    new CreateDictionaryValidator().ValidateAndThrow(payload);

    ActorId actorId = command.ActorId;

    Locale locale = new(payload.Locale);
    Dictionary dictionary = new(locale, command.TenantId, actorId);

    await _dictionaryManager.SaveAsync(dictionary, actorId, cancellationToken);

    return await _dictionaryQuerier.ReadAsync(command.Realm, dictionary, cancellationToken);
  }
}
