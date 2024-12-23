﻿using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Settings.Validators;

namespace Logitar.Portal.Application.Realms.Validators;

internal class UpdateRealmValidator : AbstractValidator<UpdateRealmPayload>
{
  public UpdateRealmValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueSlug), () => RuleFor(x => x.UniqueSlug!).Slug());
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName?.Value), () => RuleFor(x => x.DisplayName!.Value!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description?.Value), () => RuleFor(x => x.Description!.Value!).Description());

    When(x => !string.IsNullOrWhiteSpace(x.DefaultLocale?.Value), () => RuleFor(x => x.DefaultLocale!.Value!).Locale());
    When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret!).JwtSecret());
    When(x => !string.IsNullOrWhiteSpace(x.Url?.Value), () => RuleFor(x => x.Url!.Value!).Url());

    When(x => x.UniqueNameSettings != null, () => RuleFor(x => x.UniqueNameSettings!).SetValidator(new UniqueNameSettingsValidator()));
    When(x => x.PasswordSettings != null, () => RuleFor(x => x.PasswordSettings!).SetValidator(new PasswordSettingsValidator()));

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeModificationValidator());
  }
}
