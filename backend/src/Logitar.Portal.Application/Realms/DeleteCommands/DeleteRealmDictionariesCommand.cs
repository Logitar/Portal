using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Dictionaries;
using Logitar.Portal.Domain.Realms;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal record DeleteRealmDictionariesCommand(Realm Realm, ActorId ActorId) : INotification;

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
    IEnumerable<Dictionary> dictionaries = await _dictionaryRepository.LoadAsync(tenantId, cancellationToken);

    foreach (Dictionary dictionary in dictionaries)
    {
      dictionary.Delete(command.ActorId);
    }

    await _dictionaryRepository.SaveAsync(dictionaries, cancellationToken);
  }
}
