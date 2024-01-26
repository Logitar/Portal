﻿using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class ReplaceUserValidator : AbstractValidator<ReplaceUserPayload>
{
  public ReplaceUserValidator(IUserSettings userSettings)
  {
    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(userSettings.UniqueName));
  }
}
