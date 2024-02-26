﻿using FluentValidation;
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

internal class CreateSenderCommandHandler : IRequestHandler<CreateSenderCommand, Sender>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public CreateSenderCommandHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender> Handle(CreateSenderCommand command, CancellationToken cancellationToken)
  {
    CreateSenderPayload payload = command.Payload;
    new CreateSenderValidator().ValidateAndThrow(payload);

    ActorId actorId = _applicationContext.ActorId;

    EmailUnit email = new(payload.EmailAddress, isVerified: false);
    SenderAggregate sender = new(email, GetSettings(payload), _applicationContext.TenantId, actorId)
    {
      DisplayName = DisplayNameUnit.TryCreate(payload.DisplayName),
      Description = DescriptionUnit.TryCreate(payload.Description)
    };
    sender.Update(actorId);

    IEnumerable<SenderAggregate> senders = await _senderRepository.LoadAsync(sender.TenantId, cancellationToken);
    if (!senders.Any())
    {
      sender.SetDefault(actorId);
    }

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.ReadAsync(sender, cancellationToken);
  }

  private static SenderSettings GetSettings(CreateSenderPayload payload)
  {
    if (payload.SendGrid != null)
    {
      return new ReadOnlySendGridSettings(payload.SendGrid);
    }
    else if (payload.Mailgun != null)
    {
      return new ReadOnlyMailgunSettings(payload.Mailgun);
    }

    throw new ArgumentException("No sender provider settings have been specified.", nameof(payload));
  }
}
