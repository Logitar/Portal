﻿using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain;
using Logitar.Portal.Domain.Settings.Validators;

namespace Logitar.Portal.Application.Realms.Validators;

internal class CreateRealmValidator : AbstractValidator<CreateRealmPayload>
{
  public CreateRealmValidator()
  {
    When(x => x.Id.HasValue, () => RuleFor(x => x.Id!.Value).NotEmpty());

    RuleFor(x => x.UniqueSlug).Slug();
    When(x => !string.IsNullOrWhiteSpace(x.DisplayName), () => RuleFor(x => x.DisplayName!).DisplayName());
    When(x => !string.IsNullOrWhiteSpace(x.Description), () => RuleFor(x => x.Description!).Description());

    When(x => !string.IsNullOrWhiteSpace(x.DefaultLocale), () => RuleFor(x => x.DefaultLocale!).Locale());
    When(x => !string.IsNullOrWhiteSpace(x.Secret), () => RuleFor(x => x.Secret).JwtSecret());
    When(x => !string.IsNullOrWhiteSpace(x.Url), () => RuleFor(x => x.Url!).Url());

    RuleFor(x => x.UniqueNameSettings).SetValidator(new UniqueNameSettingsValidator());
    RuleFor(x => x.PasswordSettings).SetValidator(new PasswordSettingsValidator());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
