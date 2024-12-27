using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal record SetDefaultSenderCommand(Guid Id) : Activity, IRequest<SenderModel?>;

internal class SetDefaultSenderCommandHandler : IRequestHandler<SetDefaultSenderCommand, SenderModel?>
{
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public SetDefaultSenderCommandHandler(ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<SenderModel?> Handle(SetDefaultSenderCommand command, CancellationToken cancellationToken)
  {
    SenderId senderId = new(command.TenantId, new EntityId(command.Id));
    Sender? sender = await _senderRepository.LoadAsync(senderId, cancellationToken);
    if (sender == null || sender.TenantId != command.TenantId)
    {
      return null;
    }

    if (!sender.IsDefault)
    {
      List<Sender> senders = new(capacity: 2);

      ActorId actorId = command.ActorId;

      Sender? @default = await _senderRepository.LoadDefaultAsync(sender.TenantId, cancellationToken);
      if (@default != null)
      {
        @default.SetDefault(isDefault: false, actorId);
        senders.Add(@default);
      }

      sender.SetDefault(actorId);
      senders.Add(sender);

      await _senderRepository.SaveAsync(senders, cancellationToken);
    }

    return await _senderQuerier.ReadAsync(command.Realm, sender, cancellationToken);
  }
}
