using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class ResetUserPasswordValidator : AbstractValidator<ResetUserPasswordPayload>
{
  public ResetUserPasswordValidator(IUserSettings userSettings)
  {
    RuleFor(x => x.Password).SetValidator(new PasswordValidator(userSettings.Password));
  }
}
