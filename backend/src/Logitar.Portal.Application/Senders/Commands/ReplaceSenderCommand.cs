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

internal record ReplaceSenderCommand(Guid Id, ReplaceSenderPayload Payload, long? Version) : Activity, IRequest<SenderModel?>;

internal class ReplaceSenderCommandHandler : IRequestHandler<ReplaceSenderCommand, SenderModel?>
{
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public ReplaceSenderCommandHandler(ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<SenderModel?> Handle(ReplaceSenderCommand command, CancellationToken cancellationToken)
  {
    SenderId senderId = new(command.TenantId, new EntityId(command.Id));
    Sender? sender = await _senderRepository.LoadAsync(senderId, cancellationToken);
    if (sender == null || sender.TenantId != command.TenantId)
    {
      return null;
    }

    ReplaceSenderPayload payload = command.Payload;
    new ReplaceSenderValidator(sender.Provider).ValidateAndThrow(payload);

    Sender? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _senderRepository.LoadAsync(sender.Id, command.Version.Value, cancellationToken);
    }

    ActorId actorId = command.ActorId;

    switch (sender.Type)
    {
      case SenderType.Email:
        if (payload.EmailAddress == null)
        {
          throw new InvalidOperationException("The sender email address is required.");
        }
        Email email = new(payload.EmailAddress, isVerified: false);
        if (reference == null || email != reference.Email)
        {
          sender.Email = email;
        }
        DisplayName? displayName = DisplayName.TryCreate(payload.DisplayName);
        if (reference == null || displayName != reference.DisplayName)
        {
          sender.DisplayName = displayName;
        }
        break;
      case SenderType.Sms:
        if (payload.PhoneNumber == null)
        {
          throw new InvalidOperationException("The sender phone number is required.");
        }
        Phone phone = new(payload.PhoneNumber, countryCode: null, extension: null, isVerified: false);
        if (reference == null || phone != reference.Phone)
        {
          sender.Phone = phone;
        }
        break;
      default:
        throw new SenderTypeNotSupportedException(sender.Type);
    }

    Description? description = Description.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      sender.Description = description;
    }

    SetSettings(payload, sender, reference, actorId);

    sender.Update(actorId);
    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.ReadAsync(command.Realm, sender, cancellationToken);
  }

  private static void SetSettings(ReplaceSenderPayload payload, Sender sender, Sender? reference, ActorId actorId)
  {
    if (payload.SendGrid != null)
    {
      ReadOnlySendGridSettings settings = new(payload.SendGrid);
      if (reference == null || settings != reference.Settings)
      {
        sender.SetSettings(settings, actorId);
      }
    }
    if (payload.Mailgun != null)
    {
      ReadOnlyMailgunSettings settings = new(payload.Mailgun);
      if (reference == null || settings != reference.Settings)
      {
        sender.SetSettings(settings, actorId);
      }
    }
    if (payload.Twilio != null)
    {
      ReadOnlyTwilioSettings settings = new(payload.Twilio);
      if (reference == null || settings != reference.Settings)
      {
        sender.SetSettings(settings, actorId);
      }
    }
  }
}
