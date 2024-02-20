using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Senders.Validators;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal class UpdateSenderCommandHandler : IRequestHandler<UpdateSenderCommand, Sender?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public UpdateSenderCommandHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender?> Handle(UpdateSenderCommand command, CancellationToken cancellationToken)
  {
    UpdateSenderPayload payload = command.Payload;
    new UpdateSenderValidator().ValidateAndThrow(payload);

    SenderAggregate? sender = await _senderRepository.LoadAsync(command.Id, cancellationToken);
    if (sender == null || sender.TenantId != _applicationContext.TenantId)
    {
      return null;
    }

    ActorId actorId = _applicationContext.ActorId;

    if (payload.Email != null)
    {
      sender.Email = payload.Email.ToEmailUnit();
    }
    if (payload.DisplayName != null)
    {
      sender.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      sender.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    if (payload.SendGrid != null)
    {
      ReadOnlySendGridSettings settings = new(payload.SendGrid);
      sender.SetSettings(settings, actorId);
    }

    sender.Update(actorId);
    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.ReadAsync(sender, cancellationToken);
  }
}
