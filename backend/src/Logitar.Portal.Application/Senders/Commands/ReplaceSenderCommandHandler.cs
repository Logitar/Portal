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

internal class ReplaceSenderCommandHandler : IRequestHandler<ReplaceSenderCommand, Sender?>
{
  private readonly IApplicationContext _applicationContext;
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public ReplaceSenderCommandHandler(IApplicationContext applicationContext, ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _applicationContext = applicationContext;
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<Sender?> Handle(ReplaceSenderCommand command, CancellationToken cancellationToken)
  {
    ReplaceSenderPayload payload = command.Payload;
    new ReplaceSenderValidator().ValidateAndThrow(payload);

    SenderAggregate? sender = await _senderRepository.LoadAsync(command.Id, cancellationToken);
    if (sender == null || sender.TenantId != _applicationContext.TenantId)
    {
      return null;
    }
    SenderAggregate? reference = null;
    if (command.Version.HasValue)
    {
      reference = await _senderRepository.LoadAsync(sender.Id, command.Version.Value, cancellationToken);
    }

    ActorId actorId = _applicationContext.ActorId;

    EmailUnit email = new(payload.EmailAddress, isVerified: false);
    if (reference == null || email != reference.Email)
    {
      sender.Email = email;
    }
    DisplayNameUnit? displayName = DisplayNameUnit.TryCreate(payload.DisplayName);
    if (reference == null || displayName != reference.DisplayName)
    {
      sender.DisplayName = displayName;
    }
    DescriptionUnit? description = DescriptionUnit.TryCreate(payload.Description);
    if (reference == null || description != reference.Description)
    {
      sender.Description = description;
    }

    SetSettings(payload, sender, reference, actorId);

    sender.Update(actorId);
    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.ReadAsync(sender, cancellationToken);
  }

  private static void SetSettings(ReplaceSenderPayload payload, SenderAggregate sender, SenderAggregate? reference, ActorId actorId)
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
  }
}
