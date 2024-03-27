using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders;
using Logitar.Portal.Domain.Senders.Mailgun;
using Logitar.Portal.Domain.Senders.SendGrid;
using Logitar.Portal.Domain.Senders.Twilio;

namespace Logitar.Portal.Application.Senders.Validators;

internal class UpdateSenderValidator : AbstractValidator<UpdateSenderPayload>
{
  public UpdateSenderValidator(SenderType type)
  {
    switch (type)
    {
      case SenderType.Email:
        When(x => x.EmailAddress != null, () => RuleFor(x => x.EmailAddress!).SetValidator(new EmailAddressValidator()));
        RuleFor(x => x.PhoneNumber).Empty();
        When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));

        When(x => x.Mailgun != null, () => RuleFor(x => x.Mailgun!).SetValidator(new MailgunSettingsValidator()));
        When(x => x.SendGrid != null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()));
        RuleFor(x => x.Twilio).Null();
        break;
      case SenderType.Sms:
        RuleFor(x => x.EmailAddress).Empty();
        When(x => x.PhoneNumber != null, () => RuleFor(x => x.EmailAddress!).SetValidator(new PhoneNumberValidator()));
        RuleFor(x => x.DisplayName).Empty();

        RuleFor(x => x.Mailgun).Null();
        RuleFor(x => x.SendGrid).Null();
        When(x => x.Twilio != null, () => RuleFor(x => x.Twilio!).SetValidator(new TwilioSettingsValidator()));
        break;
      default:
        throw new SenderTypeNotSupportedException(type);
    }

    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));
  }
}
