using FluentValidation;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  internal class UserValidator : AbstractValidator<User>
  {
    /// <summary>
    /// TODO(fpion): WithErrorCode? (username)
    /// TODO(fpion): WithMessage? (username)
    /// </summary>
    public UserValidator(UsernameSettings usernameSettings)
    {
      RuleFor(x => x.Username).NotEmpty()
        .MaximumLength(256)
        .Must(u => usernameSettings.AllowedCharacters == null || u.All(usernameSettings.AllowedCharacters.Contains));

      RuleFor(x => x.PasswordHash).NullOrNotEmpty();

      RuleFor(x => x.Email).NullOrNotEmpty()
        .MaximumLength(256)
        .EmailAddress();

      RuleFor(x => x.FirstName).NullOrNotEmpty()
        .MaximumLength(128);

      RuleFor(x => x.LastName).NullOrNotEmpty()
        .MaximumLength(128);

      RuleFor(x => x.Locale).Locale();
    }
  }
}
