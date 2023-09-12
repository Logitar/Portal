using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class DeleteSendersCommandHandler : INotificationHandler<DeleteSendersCommand>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderRepository _senderRepository;

  public DeleteSendersCommandHandler(IApplicationContext applicationContext, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderRepository = senderRepository;
  }

  public async Task Handle(DeleteSendersCommand command, CancellationToken cancellationToken)
  {
    string tenantId = command.Realm.Id.Value;

    IEnumerable<SenderAggregate> senders = await _senderRepository.LoadAsync(tenantId, cancellationToken);
    foreach (SenderAggregate sender in senders)
    {
      sender.Delete(_applicationContext.ActorId);
    }

    await _senderRepository.SaveAsync(senders, cancellationToken);
  }
}
