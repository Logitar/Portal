using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Dictionaries.Commands;

internal class DeleteDictionariesCommandHandler : INotificationHandler<DeleteDictionariesCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly IDictionaryRepository _userRepository;

  public DeleteDictionariesCommandHandler(IApplicationContext applicationContext, IDictionaryRepository userRepository)
  {
    _applicationContext = applicationContext;
    _userRepository = userRepository;
  }

  public async Task Handle(DeleteDictionariesCommand command, CancellationToken cancellationToken)
  {
    IEnumerable<DictionaryAggregate> dictionaries = await _userRepository.LoadAsync(command.Realm, cancellationToken);
    foreach (DictionaryAggregate dictionary in dictionaries)
    {
      dictionary.Delete(_applicationContext.ActorId);
    }

    await _userRepository.SaveAsync(dictionaries, cancellationToken);
  }
}
