using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users;
using Logitar.Portal.Application.Senders.Validators;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.Mailgun;
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

    if (!string.IsNullOrWhiteSpace(payload.EmailAddress))
    {
      sender.Email = new EmailUnit(payload.EmailAddress);
    }
    if (payload.DisplayName != null)
    {
      sender.DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      sender.Description = DescriptionUnit.TryCreate(payload.Description.Value);
    }

    SetSettings(payload, sender, actorId);

    sender.Update(actorId);
    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.ReadAsync(sender, cancellationToken);
  }

  private static void SetSettings(UpdateSenderPayload payload, SenderAggregate sender, ActorId actorId)
  {
    if (payload.SendGrid != null)
    {
      ReadOnlySendGridSettings settings = new(payload.SendGrid);
      sender.SetSettings(settings, actorId);
    }
    if (payload.Mailgun != null)
    {
      ReadOnlyMailgunSettings settings = new(payload.Mailgun);
      sender.SetSettings(settings, actorId);
    }
  }
}
