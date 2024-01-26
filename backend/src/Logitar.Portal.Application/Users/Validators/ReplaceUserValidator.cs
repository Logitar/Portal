using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class ReplaceUserValidator : AbstractValidator<ReplaceUserPayload>
{
  public ReplaceUserValidator(IUserSettings userSettings)
  {
    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(userSettings.UniqueName));
    When(x => x.Password != null, () => RuleFor(x => x.Password!).SetValidator(new PasswordValidator(userSettings.Password)));

    When(x => x.Address != null, () => RuleFor(x => x.Address!).SetValidator(new AddressValidator()));
    When(x => x.Email != null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    When(x => x.Phone != null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));
  }
}
