using FluentValidation;
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

      When(x => x.PasswordHash == null, () => RuleFor(x => x.PasswordChangedAt).Null());

      RuleFor(x => x.Email)
        .MaximumLength(256)
        .EmailAddress();

      When(x => x.Email == null, () =>
      {
        RuleFor(x => x.EmailConfirmedAt).Null();
        RuleFor(x => x.EmailConfirmedById).Null();
      });

      When(x => x.PhoneNumber == null, () =>
      {
        RuleFor(x => x.PhoneNumberConfirmedAt).Null();
        RuleFor(x => x.PhoneNumberConfirmedById).Null();
      });

      RuleFor(x => x.FirstName)
        .MaximumLength(128);
      RuleFor(x => x.LastName)
        .MaximumLength(128);
      RuleFor(x => x.MiddleName)
        .MaximumLength(128);

      RuleFor(x => x.Locale)
        .Must(BeAValidCulture);
      RuleFor(x => x.Picture)
        .MaximumLength(2048)
        .Must(ValidationRules.BeAValidUrl);
    }

    private static bool BeAValidCulture(string? value) => value == null || CultureInfo.GetCultureInfo(value).LCID != 4096;
  }
}
