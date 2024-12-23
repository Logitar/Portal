﻿using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Roles;

namespace Logitar.Portal.Application.Roles.Validators;

internal class ReplaceRoleValidator : AbstractValidator<ReplaceRolePayload>
{
  public ReplaceRoleValidator(IRoleSettings roleSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(roleSettings.UniqueName);
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
