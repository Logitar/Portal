using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class SetDefaultSenderCommandHandler : IRequestHandler<SetDefaultSenderCommand, Sender?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public SetDefaultSenderCommandHandler(IApplicationContext applicationContext,
    ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender?> Handle(SetDefaultSenderCommand command, CancellationToken cancellationToken)
  {
    SenderAggregate? sender = await _senderRepository.LoadAsync(command.Id, cancellationToken);
    if (sender == null)
    {
      return null;
    }

    SenderAggregate? @default = await _senderRepository.LoadDefaultAsync(sender.TenantId, cancellationToken);
    if (@default != null)
    {
      @default.IsDefault = false;
      @default.Update(_applicationContext.ActorId);
    }

    sender.IsDefault = true;
    sender.Update(_applicationContext.ActorId);

    List<SenderAggregate> senders = new(capacity: 2);
    if (@default != null)
    {
      senders.Add(@default);
    }
    senders.Add(sender);

    await _senderRepository.SaveAsync(senders, cancellationToken);

    return await _senderQuerier.ReadAsync(sender, cancellationToken);
  }
}
