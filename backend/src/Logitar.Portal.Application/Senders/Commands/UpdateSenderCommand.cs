﻿using FluentValidation;
using Logitar.EventSourcing;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Application.Activities;
using Logitar.Portal.Application.Senders.Validators;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.Mailgun;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.Domain.Senders.Twilio;
using MediatR;

namespace Logitar.Portal.Application.Senders.Commands;

internal record UpdateSenderCommand(Guid Id, UpdateSenderPayload Payload) : Activity, IRequest<SenderModel?>;

internal class UpdateSenderCommandHandler : IRequestHandler<UpdateSenderCommand, SenderModel?>
{
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public UpdateSenderCommandHandler(ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<SenderModel?> Handle(UpdateSenderCommand command, CancellationToken cancellationToken)
  {
    SenderId senderId = new(command.TenantId, new EntityId(command.Id));
    Sender? sender = await _senderRepository.LoadAsync(senderId, cancellationToken);
    if (sender == null || sender.TenantId != command.TenantId)
    {
      return null;
    }

    UpdateSenderPayload payload = command.Payload;
    new UpdateSenderValidator(sender.Provider).ValidateAndThrow(payload);

    ActorId actorId = command.ActorId;

    if (!string.IsNullOrWhiteSpace(payload.EmailAddress))
    {
      sender.Email = new Email(payload.EmailAddress);
    }
    if (!string.IsNullOrWhiteSpace(payload.PhoneNumber))
    {
      sender.Phone = new Phone(payload.PhoneNumber);
    }
    if (payload.DisplayName != null)
    {
      sender.DisplayName = DisplayName.TryCreate(payload.DisplayName.Value);
    }
    if (payload.Description != null)
    {
      sender.Description = Description.TryCreate(payload.Description.Value);
    }

    SetSettings(payload, sender, actorId);

    sender.Update(actorId);
    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.ReadAsync(command.Realm, sender, cancellationToken);
  }

  private static void SetSettings(UpdateSenderPayload payload, Sender sender, ActorId actorId)
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
    if (payload.Twilio != null)
    {
      ReadOnlyTwilioSettings settings = new(payload.Twilio);
      sender.SetSettings(settings, actorId);
    }
  }
}
