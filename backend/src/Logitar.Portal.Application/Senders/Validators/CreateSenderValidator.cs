using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.Mailgun;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.Domain.Senders.Twilio;

namespace Logitar.Portal.Application.Senders.Validators;

internal class CreateSenderValidator : AbstractValidator<CreateSenderPayload>
{
  public CreateSenderValidator()
  {
    When(x => GetProvider(x) == null, () => RuleFor(x => x).Must(x => GetProvider(x) != null)
      .WithErrorCode(nameof(CreateSenderValidator))
      .WithMessage(x => $"Only one of the following must be provided: {nameof(x.Mailgun)}, {nameof(x.SendGrid)}, {nameof(x.Twilio)}.")
    ).Otherwise(() =>
    {
      When(IsEmailSender, () =>
      {
        RuleFor(x => x.EmailAddress).NotNull();
        When(x => x.EmailAddress != null, () => RuleFor(x => x.EmailAddress!).SetValidator(new EmailAddressValidator()));
        RuleFor(x => x.PhoneNumber).Empty();
        When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));

        When(x => x.Mailgun != null, () => RuleFor(x => x.Mailgun!).SetValidator(new MailgunSettingsValidator()));
        When(x => x.SendGrid != null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()));
      });
      When(IsSmsSender, () =>
      {
        RuleFor(x => x.EmailAddress).Empty();
        RuleFor(x => x.PhoneNumber).NotNull();
        When(x => x.PhoneNumber != null, () => RuleFor(x => x.PhoneNumber!).SetValidator(new PhoneNumberValidator()));
        RuleFor(x => x.DisplayName).Empty();

        When(x => x.Twilio != null, () => RuleFor(x => x.Twilio!).SetValidator(new TwilioSettingsValidator()));
      });
    });

    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));
  }

  private static SenderProvider? GetProvider(CreateSenderPayload payload)
  {
    List<SenderProvider> providers = new(capacity: 3);
    if (payload.Mailgun != null)
    {
      providers.Add(SenderProvider.Mailgun);
    }
    if (payload.SendGrid != null)
    {
      providers.Add(SenderProvider.SendGrid);
    }
    if (payload.Twilio != null)
    {
      providers.Add(SenderProvider.Twilio);
    }
    return providers.Count == 1 ? providers.Single() : null;
  }
  private static bool IsEmailSender(CreateSenderPayload payload) => GetProvider(payload)?.GetSenderType() == SenderType.Email;
  private static bool IsSmsSender(CreateSenderPayload payload) => GetProvider(payload)?.GetSenderType() == SenderType.Sms;
}
