using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class DeleteSenderCommandHandler : IRequestHandler<DeleteSenderCommand, Sender?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public DeleteSenderCommandHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender?> Handle(DeleteSenderCommand command, CancellationToken cancellationToken)
  {
    SenderAggregate? sender = await _senderRepository.LoadAsync(command.Id, cancellationToken);
    if (sender == null || sender.TenantId != _applicationContext.TenantId)
    {
      return null;
    }
    else if (sender.IsDefault)
    {
      IEnumerable<SenderAggregate> senders = await _senderRepository.LoadAsync(sender.TenantId, cancellationToken);
      if (senders.Count() > 1)
      {
        throw new CannotDeleteDefaultSenderException(sender);
      }
    }
    Sender result = await _senderQuerier.ReadAsync(sender, cancellationToken);

    sender.Delete(_applicationContext.ActorId);
    await _senderRepository.SaveAsync(sender, cancellationToken);

    return result;
  }
}
