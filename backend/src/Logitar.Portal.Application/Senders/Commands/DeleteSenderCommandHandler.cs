using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class DeleteSenderCommandHandler : IRequestHandler<DeleteSenderCommand, SenderModel?>
{
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public DeleteSenderCommandHandler(ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<SenderModel?> Handle(DeleteSenderCommand command, CancellationToken cancellationToken)
  {
    Sender? sender = await _senderRepository.LoadAsync(command.Id, cancellationToken);
    if (sender == null || sender.TenantId != command.TenantId)
    {
      return null;
    }
    else if (sender.IsDefault)
    {
      IEnumerable<Sender> senders = await _senderRepository.LoadAsync(sender.TenantId, cancellationToken);
      if (senders.Count() > 1)
      {
        throw new CannotDeleteDefaultSenderException(sender);
      }
    }
    SenderModel result = await _senderQuerier.ReadAsync(command.Realm, sender, cancellationToken);

    sender.Delete(command.ActorId);
    await _senderRepository.SaveAsync(sender, cancellationToken);

    return result;
  }
}
