using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal record DeleteDictionaryCommand(Guid Id) : Activity, IRequest<DictionaryModel?>;

internal class DeleteDictionaryCommandHandler : IRequestHandler<DeleteDictionaryCommand, DictionaryModel?>
{
  private readonly IDictionaryManager _dictionaryManager;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public DeleteDictionaryCommandHandler(IDictionaryManager dictionaryManager, IDictionaryRepository dictionaryRepository, IDictionaryQuerier dictionaryQuerier)
  {
    _dictionaryManager = dictionaryManager;
    _dictionaryRepository = dictionaryRepository;
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<DictionaryModel?> Handle(DeleteDictionaryCommand command, CancellationToken cancellationToken)
  {
    DictionaryId dictionaryId = new(command.TenantId, new EntityId(command.Id));
    Dictionary? dictionary = await _dictionaryRepository.LoadAsync(dictionaryId, cancellationToken);
    if (dictionary == null || dictionary.TenantId != command.TenantId)
    {
      return null;
    }
    DictionaryModel result = await _dictionaryQuerier.ReadAsync(command.Realm, dictionary, cancellationToken);

    ActorId actorId = command.ActorId;
    dictionary.Delete(actorId);
    await _dictionaryManager.SaveAsync(dictionary, actorId, cancellationToken);

    return result;
  }
}
