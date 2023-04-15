using MediatR;

namespace Logitar.Portal.Core.Senders.Commands;

internal class DeleteSendersHandler : IRequestHandler<DeleteSenders>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderRepository _senderRepository;

  public DeleteSendersHandler(IApplicationContext applicationContext, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderRepository = senderRepository;
  }

  public async Task Handle(DeleteSenders request, CancellationToken cancellationToken)
  {
    IEnumerable<SenderAggregate> senders = await _senderRepository.LoadAsync(request.Realm, cancellationToken);
    foreach (SenderAggregate sender in senders)
    {
      sender.Delete(_applicationContext.ActorId);
    }

    await _senderRepository.SaveAsync(senders, cancellationToken);
  }
}
