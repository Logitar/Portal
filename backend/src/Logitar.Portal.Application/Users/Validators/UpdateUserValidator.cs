﻿using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Identity.Core.Validators;
using Logitar.Portal.Application.Roles.Validators;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class UpdateUserValidator : AbstractValidator<UpdateUserPayload>
{
  public UpdateUserValidator(IAddressHelper addressHelper, IUserSettings userSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).UniqueName(userSettings.UniqueName));
    When(x => x.Password != null, () => RuleFor(x => x.Password!).SetValidator(new ChangePasswordValidator(userSettings.Password)));

    When(x => x.Address?.Value != null, () => RuleFor(x => x.Address!.Value!).SetValidator(new AddressValidator(addressHelper)));
    When(x => x.Email?.Value != null, () => RuleFor(x => x.Email!.Value!).SetValidator(new EmailValidator()));
    When(x => x.Phone?.Value != null, () => RuleFor(x => x.Phone!.Value!).SetValidator(new PhoneValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.FirstName?.Value), () => RuleFor(x => x.FirstName!.Value!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.MiddleName?.Value), () => RuleFor(x => x.MiddleName!.Value!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.LastName?.Value), () => RuleFor(x => x.LastName!.Value!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.Nickname?.Value), () => RuleFor(x => x.Nickname!.Value!).PersonName());

    When(x => x.Birthdate?.Value.HasValue == true, () => RuleFor(x => x.Birthdate!.Value!.Value).Past());
    When(x => !string.IsNullOrWhiteSpace(x.Gender?.Value), () => RuleFor(x => x.Gender!.Value!).Gender());
    When(x => !string.IsNullOrWhiteSpace(x.Locale?.Value), () => RuleFor(x => x.Locale!.Value!).Locale());
    When(x => !string.IsNullOrWhiteSpace(x.TimeZone?.Value), () => RuleFor(x => x.TimeZone!.Value!).TimeZone());

    When(x => !string.IsNullOrWhiteSpace(x.Picture?.Value), () => RuleFor(x => x.Picture!.Value!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Profile?.Value), () => RuleFor(x => x.Profile!.Value!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Website?.Value), () => RuleFor(x => x.Website!.Value!).Url());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeModificationValidator());
    RuleForEach(x => x.Roles).SetValidator(new RoleModificationValidator());
  }
}
