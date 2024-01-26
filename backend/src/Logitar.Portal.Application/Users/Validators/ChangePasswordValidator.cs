using FluentValidation;
using Logitar.Identity.Contracts.Settings;
using Logitar.Identity.Domain.Passwords.Validators;
using Logitar.Portal.Contracts.Users;

namespace Logitar.Portal.Application.Users.Validators;

internal class ChangePasswordValidator : AbstractValidator<ChangePasswordPayload>
{
  public ChangePasswordValidator(IPasswordSettings passwordSettings)
  {
    When(x => x.CurrentPassword != null, () => RuleFor(x => x.CurrentPassword).NotEmpty());
    RuleFor(x => x.NewPassword).SetValidator(new PasswordValidator(passwordSettings));
  }
}
