using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class DeleteSenderCommandHandler : IRequestHandler<DeleteSenderCommand, Sender?>
{
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public DeleteSenderCommandHandler(ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender?> Handle(DeleteSenderCommand command, CancellationToken cancellationToken)
  {
    SenderAggregate? sender = await _senderRepository.LoadAsync(command.Id, cancellationToken);
    if (sender == null || sender.TenantId != command.TenantId)
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
    Sender result = await _senderQuerier.ReadAsync(command.Realm, sender, cancellationToken);

    sender.Delete(command.ActorId);
    await _senderRepository.SaveAsync(sender, cancellationToken);

    return result;
  }
}
