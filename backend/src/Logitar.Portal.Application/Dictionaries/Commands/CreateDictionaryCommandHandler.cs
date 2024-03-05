using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Dictionaries.Validators;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal class CreateDictionaryCommandHandler : IRequestHandler<CreateDictionaryCommand, Dictionary>
{
  private readonly IDictionaryManager _dictionaryManager;
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public CreateDictionaryCommandHandler(IDictionaryManager dictionaryManager, IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryManager = dictionaryManager;
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<Dictionary> Handle(CreateDictionaryCommand command, CancellationToken cancellationToken)
  {
    CreateDictionaryPayload payload = command.Payload;
    new CreateDictionaryValidator().ValidateAndThrow(payload);

    ActorId actorId = command.ActorId;

    LocaleUnit locale = new(payload.Locale);
    DictionaryAggregate dictionary = new(locale, command.TenantId, actorId);

    await _dictionaryManager.SaveAsync(dictionary, actorId, cancellationToken);

    return await _dictionaryQuerier.ReadAsync(command.Realm, dictionary, cancellationToken);
  }
}
