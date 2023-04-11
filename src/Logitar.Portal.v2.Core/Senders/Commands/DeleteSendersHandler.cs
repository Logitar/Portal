using MediatR;

namespace Logitar.Portal.v2.Core.Senders.Commands;

internal class DeleteSendersHandler : IRequestHandler<DeleteSenders>
{
  private readonly ICurrentActor _currentActor;
  private readonly ISenderRepository _senderRepository;

  public DeleteSendersHandler(ICurrentActor currentActor, ISenderRepository senderRepository)
  {
    _currentActor = currentActor;
    _senderRepository = senderRepository;
  }

  public async Task Handle(DeleteSenders request, CancellationToken cancellationToken)
  {
    IEnumerable<SenderAggregate> senders = await _senderRepository.LoadAsync(request.Realm, cancellationToken);
    foreach (SenderAggregate sender in senders)
    {
      sender.Delete(_currentActor.Id);
    }

    await _senderRepository.SaveAsync(senders, cancellationToken);
  }
}
