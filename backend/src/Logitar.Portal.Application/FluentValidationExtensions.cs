using FluentValidation;
using Logitar.Portal.Domain.Users;
using System.Globalization;

namespace Logitar.Portal.Application
{
  public static class FluentValidationExtensions
  {
    public static IRuleBuilder<T, string?> Alias<T>(this IRuleBuilder<T, string?> rules)
    {
      return rules.Must(a => a == null || a.Split('-').All(w => !string.IsNullOrEmpty(w) && w.All(char.IsLetterOrDigit)))
        .WithErrorCode("AliasValidator")
        .WithMessage("'{PropertyName}' must be composed of non-empty alphanumeric words, separated by hyphens '-'.");
    }

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

    public static IRuleBuilder<T, string?> Purpose<T>(this IRuleBuilder<T, string?> rules)
    {
      return rules.Must(p => p == null || p.Split('_').All(w => !string.IsNullOrEmpty(w) && w.All(char.IsLetter)))
        .WithErrorCode("PurposeValidator")
        .WithMessage("'{PropertyName}' must be composed of non-empty letter-only words, separated by underscores '_'.");
    }

    public static IRuleBuilder<T, string?> Url<T>(this IRuleBuilder<T, string?> rules)
    {
      return rules.Must(u => u == null || Uri.IsWellFormedUriString(u, UriKind.RelativeOrAbsolute))
        .WithErrorCode("UrlValidator")
        .WithMessage("'{PropertyName}' must be a well formed URL.");
    }

    public static IRuleBuilder<T, string?> Username<T>(this IRuleBuilder<T, string?> rules, UsernameSettings usernameSettings)
    {
      return rules.Must(u => u == null || usernameSettings.AllowedCharacters == null || u.All(usernameSettings.AllowedCharacters.Contains))
        .WithErrorCode("UsernameValidator")
        .WithMessage($"'{{PropertyName}}' can only contain the following characters: {usernameSettings.AllowedCharacters}");
    }
  }
}
