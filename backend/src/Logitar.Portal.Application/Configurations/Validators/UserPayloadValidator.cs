using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Application.Configurations.Validators;

internal class UserPayloadValidator : AbstractValidator<UserPayload>
{
  public UserPayloadValidator(IUserSettings userSettings)
  {
    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(userSettings.UniqueName));
    RuleFor(x => x.Password).SetValidator(new PasswordValidator(userSettings.Password));

    When(x => x.Email != null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));

    When(x => !string.IsNullOrWhiteSpace(x.FirstName), () => RuleFor(x => x.FirstName!).SetValidator(new PersonNameValidator()));
    When(x => !string.IsNullOrWhiteSpace(x.LastName), () => RuleFor(x => x.LastName!).SetValidator(new PersonNameValidator()));
  }
}
