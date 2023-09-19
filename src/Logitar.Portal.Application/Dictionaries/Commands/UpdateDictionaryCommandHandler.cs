using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal class UpdateDictionaryCommandHandler : IRequestHandler<UpdateDictionaryCommand, Dictionary?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly IDictionaryRepository _dictionaryRepository;

  public UpdateDictionaryCommandHandler(IApplicationContext applicationContext,
    IDictionaryQuerier dictionaryQuerier, IDictionaryRepository dictionaryRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task<Dictionary?> Handle(UpdateDictionaryCommand command, CancellationToken cancellationToken)
  {
    DictionaryAggregate? dictionary = await _dictionaryRepository.LoadAsync(command.Id, cancellationToken);
    if (dictionary == null)
    {
      return null;
    }

    UpdateDictionaryPayload payload = command.Payload;

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

    dictionary.Update(_applicationContext.ActorId);

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);

    return await _dictionaryQuerier.ReadAsync(dictionary, cancellationToken);
  }
}
