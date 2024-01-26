using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using Logitar.Identity.Domain.Shared;
using Logitar.Identity.Domain.Users.Validators;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class CreateUserValidator : AbstractValidator<CreateUserPayload>
{
  public CreateUserValidator(IUserSettings userSettings)
  {
    When(x => x.Id != null, () => RuleFor(x => x.Id!).SetValidator(new IdentifierValidator()));

    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(userSettings.UniqueName));
    When(x => x.Password != null, () => RuleFor(x => x.Password!).SetValidator(new PasswordValidator(userSettings.Password)));

    When(x => x.Address != null, () => RuleFor(x => x.Address!).SetValidator(new AddressValidator()));
    When(x => x.Email != null, () => RuleFor(x => x.Email!).SetValidator(new EmailValidator()));
    When(x => x.Phone != null, () => RuleFor(x => x.Phone!).SetValidator(new PhoneValidator()));
  }
}
