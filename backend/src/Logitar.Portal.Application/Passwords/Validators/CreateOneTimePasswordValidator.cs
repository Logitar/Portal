﻿using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Passwords;

namespace Logitar.Portal.Application.Passwords.Validators;

internal class CreateOneTimePasswordValidator : AbstractValidator<CreateOneTimePasswordPayload>
{
  public CreateOneTimePasswordValidator()
  {
    When(x => x.Id.HasValue, () => RuleFor(x => x.Id!.Value).NotEmpty());

    RuleFor(x => x.Characters).NotEmpty();
    RuleFor(x => x.Length).GreaterThan(0);

    When(x => x.ExpiresOn.HasValue, () => RuleFor(x => x.ExpiresOn!.Value).Future());
    When(x => x.MaximumAttempts.HasValue, () => RuleFor(x => x.MaximumAttempts!.Value).GreaterThan(0));

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
