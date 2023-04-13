using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Commands;

internal class DeleteDictionaryHandler : IRequestHandler<DeleteDictionary, Dictionary>
{
  private readonly ICurrentActor _currentActor;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly IDictionaryRepository _dictionaryRepository;

  public DeleteDictionaryHandler(ICurrentActor currentActor,
    IDictionaryQuerier dictionaryQuerier,
    IDictionaryRepository dictionaryRepository)
  {
    _currentActor = currentActor;
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task<Dictionary> Handle(DeleteDictionary request, CancellationToken cancellationToken)
  {
    DictionaryAggregate dictionary = await _dictionaryRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<DictionaryAggregate>(request.Id);
    Dictionary output = await _dictionaryQuerier.GetAsync(dictionary, cancellationToken);

    dictionary.Delete(_currentActor.Id);

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);

    return output;
  }
}
