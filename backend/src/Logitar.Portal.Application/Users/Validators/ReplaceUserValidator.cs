using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class ReplaceUserValidator : AbstractValidator<ReplaceUserPayload>
{
  public ReplaceUserValidator(IUserSettings userSettings)
  {
    RuleFor(x => x.UniqueName).SetValidator(new UniqueNameValidator(userSettings.UniqueName));
    When(x => x.Password != null, () => RuleFor(x => x.Password!).SetValidator(new PasswordValidator(userSettings.Password)));
  }
}
