using MediatR;

namespace Logitar.Portal.Core.Dictionaries.Commands;

internal class DeleteDictionariesHandler : IRequestHandler<DeleteDictionaries>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryRepository _dictionaryRepository;

  public DeleteDictionariesHandler(IApplicationContext applicationContext, IDictionaryRepository dictionaryRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task Handle(DeleteDictionaries request, CancellationToken cancellationToken)
  {
    IEnumerable<DictionaryAggregate> dictionaries = await _dictionaryRepository.LoadAsync(request.Realm, cancellationToken);
    foreach (DictionaryAggregate dictionary in dictionaries)
    {
      dictionary.Delete(_applicationContext.ActorId);
    }

    await _dictionaryRepository.SaveAsync(dictionaries, cancellationToken);
  }
}
