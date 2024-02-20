using Logitar.EventSourcing;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class SetDefaultSenderCommandHandler : IRequestHandler<SetDefaultSenderCommand, Sender?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public SetDefaultSenderCommandHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender?> Handle(SetDefaultSenderCommand command, CancellationToken cancellationToken)
  {
    SenderAggregate? sender = await _senderRepository.LoadAsync(command.Id, cancellationToken);
    if (sender == null || sender.TenantId != _applicationContext.TenantId)
    {
      return null;
    }

    if (!sender.IsDefault)
    {
      List<SenderAggregate> senders = new(capacity: 2);

      ActorId actorId = _applicationContext.ActorId;

      SenderAggregate? @default = await _senderRepository.LoadDefaultAsync(sender.TenantId, cancellationToken);
      if (@default != null)
      {
        @default.SetDefault(isDefault: false, actorId);
        senders.Add(@default);
      }

      sender.SetDefault(actorId);
      senders.Add(sender);

      await _senderRepository.SaveAsync(senders, cancellationToken);
    }

    return await _senderQuerier.ReadAsync(sender, cancellationToken);
  }
}
