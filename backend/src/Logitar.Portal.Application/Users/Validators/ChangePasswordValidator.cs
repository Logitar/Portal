﻿using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class ChangePasswordValidator : AbstractValidator<ChangePasswordPayload>
{
  public ChangePasswordValidator(IPasswordSettings passwordSettings)
  {
    When(x => x.Current != null, () => RuleFor(x => x.Current).NotEmpty());
    RuleFor(x => x.New).SetValidator(new PasswordValidator(passwordSettings));
  }
}
