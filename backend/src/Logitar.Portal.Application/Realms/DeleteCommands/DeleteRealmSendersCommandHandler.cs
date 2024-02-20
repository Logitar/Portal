using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Realms.DeleteCommands;

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
    IEnumerable<SenderAggregate> senders = await _senderRepository.LoadAsync(tenantId, cancellationToken);

    foreach (SenderAggregate sender in senders)
    {
      sender.Delete(command.ActorId);
    }

    await _senderRepository.SaveAsync(senders, cancellationToken);
  }
}
