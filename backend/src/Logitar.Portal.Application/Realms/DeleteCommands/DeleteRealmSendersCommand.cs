using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

internal record DeleteRealmSendersCommand(Realm Realm, ActorId ActorId) : INotification;

internal class DeleteRealmSendersCommandHandler : INotificationHandler<DeleteRealmSendersCommand>
{
  private readonly ISenderRepository _senderRepository;

  public DeleteRealmSendersCommandHandler(ISenderRepository senderRepository)
  {
    _senderRepository = senderRepository;
  }

  public async Task Handle(DeleteRealmSendersCommand command, CancellationToken cancellationToken)
  {
    TenantId tenantId = new(command.Realm.Id.Value);
    IEnumerable<Sender> senders = await _senderRepository.LoadAsync(tenantId, cancellationToken);

    foreach (Sender sender in senders)
    {
      sender.Delete(command.ActorId);
    }

    await _senderRepository.SaveAsync(senders, cancellationToken);
  }
}
