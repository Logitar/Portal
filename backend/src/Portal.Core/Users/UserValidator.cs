using FluentValidation;
using Microsoft.Extensions.Options;
using Portal.Core.Settings;
using System.Globalization;

namespace Portal.Core.Users
{
  internal class UserValidator : AbstractValidator<User>
  {
    public UserValidator(UserSettings settings)
    {
      ArgumentNullException.ThrowIfNull(settings);

      RuleFor(x => x.Username)
        .NotEmpty()
        .MaximumLength(256)
        .Must(x => x.All(c => settings.AllowedUserNameCharacters.Contains(c)));

      RuleFor(x => x.Email)
        .MaximumLength(256)
        .EmailAddress();

      RuleFor(x => x.FirstName)
        .MaximumLength(128);
      RuleFor(x => x.LastName)
        .MaximumLength(128);
      RuleFor(x => x.MiddleName)
        .MaximumLength(128);

      RuleFor(x => x.Locale)
        .Must(BeCulture);
      RuleFor(x => x.Picture)
        .Must(ValidationRules.BeUrl);
    }

    private static bool BeCulture(string? value) => value == null || CultureInfo.GetCultureInfo(value).LCID != 4096;
  }
}
