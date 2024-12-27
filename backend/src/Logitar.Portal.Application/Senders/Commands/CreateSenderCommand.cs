using FluentValidation;
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

internal record CreateSenderCommand(CreateSenderPayload Payload) : Activity, IRequest<SenderModel>;

internal class CreateSenderCommandHandler : IRequestHandler<CreateSenderCommand, SenderModel>
{
  private readonly ISenderQuerier _senderQuerier;
  private readonly ISenderRepository _senderRepository;

  public CreateSenderCommandHandler(ISenderQuerier senderQuerier, ISenderRepository senderRepository)
  {
    _senderQuerier = senderQuerier;
    _senderRepository = senderRepository;
  }

  public async Task<SenderModel> Handle(CreateSenderCommand command, CancellationToken cancellationToken)
  {
    CreateSenderPayload payload = command.Payload;
    new CreateSenderValidator().ValidateAndThrow(payload);

    ActorId actorId = command.ActorId;

    Sender sender;
    SenderSettings settings = GetSettings(payload);
    SenderType type = settings.Provider.GetSenderType();
    switch (type)
    {
      case SenderType.Email:
        if (payload.EmailAddress == null)
        {
          throw new InvalidOperationException("The sender email address is required.");
        }
        Email email = new(payload.EmailAddress, isVerified: false);
        sender = new(email, settings, command.TenantId, actorId)
        {
          DisplayName = DisplayName.TryCreate(payload.DisplayName)
        };
        break;
      case SenderType.Sms:
        if (payload.PhoneNumber == null)
        {
          throw new InvalidOperationException("The sender phone number is required.");
        }
        Phone phone = new(payload.PhoneNumber, countryCode: null, extension: null, isVerified: false);
        sender = new(phone, settings, command.TenantId, actorId);
        break;
      default:
        throw new SenderTypeNotSupportedException(type);
    }

    sender.Description = Description.TryCreate(payload.Description);
    sender.Update(actorId);

    IEnumerable<Sender> senders = await _senderRepository.LoadAsync(sender.TenantId, cancellationToken);
    if (!senders.Any())
    {
      sender.SetDefault(actorId);
    }

    await _senderRepository.SaveAsync(sender, cancellationToken);

    return await _senderQuerier.ReadAsync(command.Realm, sender, cancellationToken);
  }

  private static SenderSettings GetSettings(CreateSenderPayload payload)
  {
    List<SenderSettings> settings = new(capacity: 3);
    if (payload.SendGrid != null)
    {
      settings.Add(new ReadOnlySendGridSettings(payload.SendGrid));
    }
    if (payload.Mailgun != null)
    {
      settings.Add(new ReadOnlyMailgunSettings(payload.Mailgun));
    }
    if (payload.Twilio != null)
    {
      settings.Add(new ReadOnlyTwilioSettings(payload.Twilio));
    }

    if (settings.Count < 1)
    {
      throw new ArgumentException("No sender provider settings has been specified.", nameof(payload));
    }
    else if (settings.Count > 1)
    {
      throw new ArgumentException("Multiple sender provider settings have been specified.", nameof(payload));
    }

    return settings.Single();
  }
}
