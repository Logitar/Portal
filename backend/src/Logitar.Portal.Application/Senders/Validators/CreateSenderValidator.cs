﻿using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Portal.Contracts.Senders;
using Logitar.Portal.Domain.Senders.Mailgun;
using Logitar.Portal.Domain.Senders.SendGrid;

namespace Logitar.Portal.Application.Senders.Validators;

internal class CreateSenderValidator : AbstractValidator<CreateSenderPayload>
{
  public CreateSenderValidator()
  {
    RuleFor(x => x.Email).SetValidator(new EmailValidator());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));

    RuleFor(x => x).Must(x => x.Mailgun != null || x.SendGrid != null)
      .WithErrorCode(nameof(CreateSenderValidator))
      .WithMessage(x => $"At least one of the following must be provided: {nameof(x.Mailgun)}, {nameof(x.SendGrid)}.");
    When(x => x.Mailgun != null, () => RuleFor(x => x.Mailgun!).SetValidator(new MailgunSettingsValidator()));
    When(x => x.SendGrid != null, () => RuleFor(x => x.SendGrid!).SetValidator(new SendGridSettingsValidator()));
  }
}
