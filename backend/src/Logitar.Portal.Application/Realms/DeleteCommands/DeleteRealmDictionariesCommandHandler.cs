using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Dictionaries;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal class DeleteRealmDictionariesCommandHandler : INotificationHandler<DeleteRealmDictionariesCommand>
{
  private readonly IDictionaryRepository _dictionaryRepository;

  public DeleteRealmDictionariesCommandHandler(IDictionaryRepository dictionaryRepository)
  {
    _dictionaryRepository = dictionaryRepository;
  }

  public async Task Handle(DeleteRealmDictionariesCommand command, CancellationToken cancellationToken)
  {
    TenantId tenantId = new(command.Realm.Id.Value);
    IEnumerable<DictionaryAggregate> dictionaries = await _dictionaryRepository.LoadAsync(tenantId, cancellationToken);

    foreach (DictionaryAggregate dictionary in dictionaries)
    {
      dictionary.Delete(command.ActorId);
    }

    await _dictionaryRepository.SaveAsync(dictionaries, cancellationToken);
  }
}
