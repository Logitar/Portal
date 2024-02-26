using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal class DeleteDictionaryCommandHandler : IRequestHandler<DeleteDictionaryCommand, Dictionary?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryManager _dictionaryManager;
  private readonly IDictionaryRepository _dictionaryRepository;
  private readonly IDictionaryQuerier _dictionaryQuerier;

  public DeleteDictionaryCommandHandler(IApplicationContext applicationContext,
    IDictionaryManager dictionaryManager, IDictionaryRepository dictionaryRepository, IDictionaryQuerier dictionaryQuerier)
  {
    _applicationContext = applicationContext;
    _dictionaryManager = dictionaryManager;
    _dictionaryRepository = dictionaryRepository;
    _dictionaryQuerier = dictionaryQuerier;
  }

  public async Task<Dictionary?> Handle(DeleteDictionaryCommand command, CancellationToken cancellationToken)
  {
    DictionaryAggregate? dictionary = await _dictionaryRepository.LoadAsync(command.Id, cancellationToken);
    if (dictionary == null || dictionary.TenantId != _applicationContext.TenantId)
    {
      return null;
    }
    Dictionary result = await _dictionaryQuerier.ReadAsync(dictionary, cancellationToken);

    ActorId actorId = _applicationContext.ActorId;
    dictionary.Delete(actorId);
    await _dictionaryManager.SaveAsync(dictionary, actorId, cancellationToken);

    return result;
  }
}
