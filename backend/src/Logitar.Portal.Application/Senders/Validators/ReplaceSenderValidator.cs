using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;

namespace Logitar.Portal.Application.Senders.Validators;

internal class ReplaceSenderValidator : AbstractValidator<ReplaceSenderPayload>
{
  public ReplaceSenderValidator()
  {
    RuleFor(x => x.Email).SetValidator(new EmailValidator());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));

    RuleFor(x => x.SendGrid).SetValidator(new SendGridSettingsValidator());
  }
}
