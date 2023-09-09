using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal class ReplaceDictionaryCommandHandler : IRequestHandler<ReplaceDictionaryCommand, Dictionary?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly IDictionaryRepository _dictionaryRepository;

  public ReplaceDictionaryCommandHandler(IApplicationContext applicationContext,
    IDictionaryQuerier dictionaryQuerier, IDictionaryRepository dictionaryRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task<Dictionary?> Handle(ReplaceDictionaryCommand command, CancellationToken cancellationToken)
  {
    DictionaryAggregate? dictionary = await _dictionaryRepository.LoadAsync(command.Id, cancellationToken);
    if (dictionary == null)
    {
      return null;
    }

    DictionaryAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _dictionaryRepository.LoadAsync(dictionary.Id, command.Version.Value, cancellationToken);
    }

    ReplaceDictionaryPayload payload = command.Payload;

    HashSet<string> entryKeys = payload.Entries.Select(x => x.Key.Trim()).ToHashSet();
    foreach (string key in dictionary.Entries.Keys)
    {
      if (!entryKeys.Contains(key) && (reference == null || reference.Entries.ContainsKey(key)))
      {
        dictionary.RemoveEntry(key);
      }
    }
    foreach (DictionaryEntry entry in payload.Entries)
    {
      dictionary.SetEntry(entry.Key, entry.Value);
    }

    dictionary.Update(_applicationContext.ActorId);

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);

    return await _dictionaryQuerier.ReadAsync(dictionary, cancellationToken);
  }
}
