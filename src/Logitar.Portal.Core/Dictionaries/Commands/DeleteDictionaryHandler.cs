using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Commands;

internal class DeleteDictionaryHandler : IRequestHandler<DeleteDictionary, Dictionary>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly IDictionaryRepository _dictionaryRepository;

  public DeleteDictionaryHandler(IApplicationContext applicationContext,
    IDictionaryQuerier dictionaryQuerier,
    IDictionaryRepository dictionaryRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task<Dictionary> Handle(DeleteDictionary request, CancellationToken cancellationToken)
  {
    DictionaryAggregate dictionary = await _dictionaryRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<DictionaryAggregate>(request.Id);
    Dictionary output = await _dictionaryQuerier.GetAsync(dictionary, cancellationToken);

    dictionary.Delete(_applicationContext.ActorId);

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);

    return output;
  }
}
