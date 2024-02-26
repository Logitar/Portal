using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Portal.Application.Roles.Validators;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class UpdateUserValidator : AbstractValidator<UpdateUserPayload>
{
  public UpdateUserValidator(IUserSettings userSettings)
  {
    When(x => !string.IsNullOrWhiteSpace(x.UniqueName), () => RuleFor(x => x.UniqueName!).SetValidator(new UniqueNameValidator(userSettings.UniqueName)));
    When(x => x.Password != null, () => RuleFor(x => x.Password!).SetValidator(new ChangePasswordValidator(userSettings.Password)));

    When(x => x.Address?.Value != null, () => RuleFor(x => x.Address!.Value!).SetValidator(new AddressValidator()));
    When(x => x.Email?.Value != null, () => RuleFor(x => x.Email!.Value!).SetValidator(new EmailValidator()));
    When(x => x.Phone?.Value != null, () => RuleFor(x => x.Phone!.Value!).SetValidator(new PhoneValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.FirstName?.Value), () => RuleFor(x => x.FirstName!.Value!).SetValidator(new PersonNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.MiddleName?.Value), () => RuleFor(x => x.MiddleName!.Value!).SetValidator(new PersonNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.LastName?.Value), () => RuleFor(x => x.LastName!.Value!).SetValidator(new PersonNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Nickname?.Value), () => RuleFor(x => x.Nickname!.Value!).SetValidator(new PersonNameValidator()));

    When(x => x.Birthdate?.Value.HasValue == true, () => RuleFor(x => x.Birthdate!.Value!.Value).SetValidator(new BirthdateValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Gender?.Value), () => RuleFor(x => x.Gender!.Value!).SetValidator(new GenderValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Locale?.Value), () => RuleFor(x => x.Locale!.Value!).SetValidator(new LocaleValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.TimeZone?.Value), () => RuleFor(x => x.TimeZone!.Value!).SetValidator(new TimeZoneValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Picture?.Value), () => RuleFor(x => x.Picture!.Value!).SetValidator(new UrlValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Profile?.Value), () => RuleFor(x => x.Profile!.Value!).SetValidator(new UrlValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Website?.Value), () => RuleFor(x => x.Website!.Value!).SetValidator(new UrlValidator()));

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeModificationValidator());
    RuleForEach(x => x.Roles).SetValidator(new RoleModificationValidator());
  }
}
