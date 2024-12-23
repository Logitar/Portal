using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Core;
using Logitar.Identity.Core.Users;
using Logitar.Identity.Core.Validators;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class ReplaceUserValidator : AbstractValidator<ReplaceUserPayload>
{
  public ReplaceUserValidator(IAddressHelper addressHelper, IUserSettings userSettings)
  {
    RuleFor(x => x.UniqueName).UniqueName(userSettings.UniqueName);
    When(x => x.Password != null, () => RuleFor(x => x.Password!).Password(userSettings.Password));

    When(x => x.Address != null, () => RuleFor(x => x.Address!).SetValidator(new AddressValidator(addressHelper)));
    When(x => x.Email != null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    When(x => x.Phone != null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.FirstName), () => RuleFor(x => x.FirstName!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.MiddleName), () => RuleFor(x => x.MiddleName!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.LastName), () => RuleFor(x => x.LastName!).PersonName());
    When(x => !string.IsNullOrWhiteSpace(x.Nickname), () => RuleFor(x => x.Nickname!).PersonName());

    When(x => x.Birthdate.HasValue, () => RuleFor(x => x.Birthdate!.Value).Past());
    When(x => !string.IsNullOrWhiteSpace(x.Gender), () => RuleFor(x => x.Gender!).Gender());
    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());
    When(x => !string.IsNullOrWhiteSpace(x.TimeZone), () => RuleFor(x => x.TimeZone!).TimeZone());

    When(x => !string.IsNullOrWhiteSpace(x.Picture), () => RuleFor(x => x.Picture!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Profile), () => RuleFor(x => x.Profile!).Url());
    When(x => !string.IsNullOrWhiteSpace(x.Website), () => RuleFor(x => x.Website!).Url());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
