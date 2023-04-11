using MediatR;

namespace Logitar.Portal.v2.Core.Dictionaries.Commands;

internal class DeleteDictionariesHandler : IRequestHandler<DeleteDictionaries>
{
  private readonly ICurrentActor _currentActor;
  private readonly IDictionaryRepository _dictionaryRepository;

  public DeleteDictionariesHandler(ICurrentActor currentActor, IDictionaryRepository dictionaryRepository)
  {
    _currentActor = currentActor;
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task Handle(DeleteDictionaries request, CancellationToken cancellationToken)
  {
    IEnumerable<DictionaryAggregate> dictionaries = await _dictionaryRepository.LoadAsync(request.Realm, cancellationToken);
    foreach (DictionaryAggregate dictionary in dictionaries)
    {
      dictionary.Delete(_currentActor.Id);
    }

    await _dictionaryRepository.SaveAsync(dictionaries, cancellationToken);
  }
}
