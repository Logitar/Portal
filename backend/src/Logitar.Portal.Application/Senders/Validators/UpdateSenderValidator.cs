using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders.SendGrid;

namespace Logitar.Portal.Application.Senders.Validators;

internal class UpdateSenderValidator : AbstractValidator<UpdateSenderPayload>
{
  public UpdateSenderValidator()
  {
    When(x => x.Email != null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));

    When(x => x.SendGrid != null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()));
  }
}
