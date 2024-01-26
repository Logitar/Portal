using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class CreateUserValidator : AbstractValidator<CreateUserPayload>
{
  public CreateUserValidator(IUserSettings userSettings)
  {
    When(x => x.Id != null, () => RuleFor(x => x.Id!).SetValidator(new IdentifierValidator()));

    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(userSettings.UniqueName));
    When(x => x.Password != null, () => RuleFor(x => x.Password!).SetValidator(new PasswordValidator(userSettings.Password)));
  }
}
