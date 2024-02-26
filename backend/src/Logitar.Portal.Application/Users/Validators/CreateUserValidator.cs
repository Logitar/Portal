using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class CreateUserValidator : AbstractValidator<CreateUserPayload>
{
  public CreateUserValidator(IUserSettings userSettings)
  {
    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(userSettings.UniqueName));
    When(x => x.Password != null, () => RuleFor(x => x.Password!).SetValidator(new PasswordValidator(userSettings.Password)));

    When(x => x.Address != null, () => RuleFor(x => x.Address!).SetValidator(new AddressValidator()));
    When(x => x.Email != null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    When(x => x.Phone != null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.FirstName), () => RuleFor(x => x.FirstName!).SetValidator(new PersonNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.MiddleName), () => RuleFor(x => x.MiddleName!).SetValidator(new PersonNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.LastName), () => RuleFor(x => x.LastName!).SetValidator(new PersonNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Nickname), () => RuleFor(x => x.Nickname!).SetValidator(new PersonNameValidator()));

    When(x => x.Birthdate.HasValue, () => RuleFor(x => x.Birthdate!.Value).SetValidator(new BirthdateValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Gender), () => RuleFor(x => x.Gender!).SetValidator(new GenderValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).SetValidator(new LocaleValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.TimeZone), () => RuleFor(x => x.TimeZone!).SetValidator(new TimeZoneValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.Picture), () => RuleFor(x => x.Picture!).SetValidator(new UrlValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Profile), () => RuleFor(x => x.Profile!).SetValidator(new UrlValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.Website), () => RuleFor(x => x.Website!).SetValidator(new UrlValidator()));

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
    RuleForEach(x => x.CustomIdentifiers).SetValidator(new CustomIdentifierContractValidator());
  }
}
