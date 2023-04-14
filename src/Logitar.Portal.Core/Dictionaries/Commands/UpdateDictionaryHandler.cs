using Logitar.Portal.Contracts.Dictionaries;
using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Commands;

internal class UpdateDictionaryHandler : IRequestHandler<UpdateDictionary, Dictionary>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly IDictionaryRepository _dictionaryRepository;

  public UpdateDictionaryHandler(IApplicationContext applicationContext,
    IDictionaryQuerier dictionaryQuerier,
    IDictionaryRepository dictionaryRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task<Dictionary> Handle(UpdateDictionary request, CancellationToken cancellationToken)
  {
    DictionaryAggregate dictionary = await _dictionaryRepository.LoadAsync(request.Id, cancellationToken)
      ?? throw new AggregateNotFoundException<DictionaryAggregate>(request.Id);

    UpdateDictionaryInput input = request.Input;

    dictionary.Update(_applicationContext.ActorId, input.Entries?.ToDictionary());

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);

    return await _dictionaryQuerier.GetAsync(dictionary, cancellationToken);
  }
}
