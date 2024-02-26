using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders.Mailgun;
using Logitar.Portal.Domain.Senders.SendGrid;

namespace Logitar.Portal.Application.Senders.Validators;

internal class UpdateSenderValidator : AbstractValidator<UpdateSenderPayload>
{
  public UpdateSenderValidator()
  {
    When(x => x.EmailAddress != null, () => RuleFor(x => x.EmailAddress!).SetValidator(new EmailAddressValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));

    When(x => x.Mailgun != null, () => RuleFor(x => x.Mailgun!).SetValidator(new MailgunSettingsValidator()));
    When(x => x.SendGrid != null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()));
  }
}
