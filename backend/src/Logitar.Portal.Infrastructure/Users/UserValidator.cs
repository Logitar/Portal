using FluentValidation;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class UserValidator : AbstractValidator<User>
  {
    public UserValidator(UsernameSettings settings)
    {
      IRuleBuilderOptions<User, string> username = RuleFor(x => x.Username)
        .NotEmpty()
        .MaximumLength(256);
      if (settings.AllowedCharacters != null)
      {
        username.Must(u => u.All(c => settings.AllowedCharacters.Contains(c)));
        // TODO(fpion): WithErrorCode?
        // TODO(fpion): WithMessage?
      }

      RuleFor(x => x.PasswordHash).NullOrNotEmpty();

      RuleFor(x => x.Email)
        .NullOrNotEmpty()
        .MaximumLength(256)
        .EmailAddress();

      RuleFor(x => x.PhoneNumber).NullOrNotEmpty();

      RuleFor(x => x.FirstName)
        .NullOrNotEmpty()
        .MaximumLength(128);

      RuleFor(x => x.MiddleName)
        .NullOrNotEmpty()
        .MaximumLength(128);

      RuleFor(x => x.LastName)
        .NullOrNotEmpty()
        .MaximumLength(128);

      RuleFor(x => x.Locale).Locale();

      RuleFor(x => x.Picture)
        .NullOrNotEmpty()
        .MaximumLength(2048)
        .Uri();
    }
  }
}
