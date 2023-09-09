using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal class DeleteDictionaryCommandHandler : IRequestHandler<DeleteDictionaryCommand, Dictionary?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryQuerier _dictionaryQuerier;
  private readonly IDictionaryRepository _dictionaryRepository;

  public DeleteDictionaryCommandHandler(IApplicationContext applicationContext,
    IDictionaryQuerier dictionaryQuerier, IDictionaryRepository dictionaryRepository)
  {
    _applicationContext = applicationContext;
    _dictionaryQuerier = dictionaryQuerier;
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task<Dictionary?> Handle(DeleteDictionaryCommand command, CancellationToken cancellationToken)
  {
    DictionaryAggregate? dictionary = await _dictionaryRepository.LoadAsync(command.Id, cancellationToken);
    if (dictionary == null)
    {
      return null;
    }
    Dictionary result = await _dictionaryQuerier.ReadAsync(dictionary, cancellationToken);

    dictionary.Delete(_applicationContext.ActorId);

    await _dictionaryRepository.SaveAsync(dictionary, cancellationToken);

    return result;
  }
}
