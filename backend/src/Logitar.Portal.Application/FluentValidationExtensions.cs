using FluentValidation;
using Logitar.Portal.Domain.Users;
using System.Globalization;

namespace Logitar.Portal.Application
{
  public static class FluentValidationExtensions
  {
    public static IRuleBuilder<T, CultureInfo?> Locale<T>(this IRuleBuilder<T, CultureInfo?> rules)
    {
      return rules.Must(c => c == null || (!string.IsNullOrWhiteSpace(c.Name) && c.LCID != 4096))
        .WithErrorCode("LocaleValidator")
        .WithMessage("'{PropertyName}' is not a valid locale. Valid locales have a non-empty name and a LCID different from 4096.");
    }

    public static IRuleBuilder<T, string?> NullOrNotEmpty<T>(this IRuleBuilder<T, string?> rules)
    {
      return rules.Must(s => s == null || !string.IsNullOrWhiteSpace(s))
        .WithErrorCode("NullOrNotEmptyValidator")
        .WithMessage("'{PropertyName}' must have a null or non-empty value. It cannot be an empty or white space string.");
    }

    public static IRuleBuilder<T, string> Username<T>(this IRuleBuilder<T, string> rules, UsernameSettings usernameSettings)
    {
      return rules.Must(u => usernameSettings.AllowedCharacters == null || u.All(usernameSettings.AllowedCharacters.Contains))
        .WithErrorCode("UsernameValidator")
        .WithMessage($"'{{PropertyName}}' can only contain the following characters: {usernameSettings.AllowedCharacters}");
    }
  }
}
