using FluentValidation;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Users.Contact;
using System.Globalization;

namespace Logitar.Portal.v2.Core;

internal static class FluentValidationExtensions
{
  public static IRuleBuilder<T, string?> Alias<T>(this IRuleBuilder<T, string?> ruleBuilder)
  {
    return ruleBuilder.Must(BeAValidAlias).WithErrorCode("AliasValidator")
      .WithMessage("'{PropertyName}' must be composed of non-empty alphanumeric words separated by hyphens.");
  }
  private static bool BeAValidAlias(string? alias) => alias == null
    || alias.Split('-').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit));

  public static IRuleBuilder<T, string?> Country<T>(this IRuleBuilder<T, string?> ruleBuilder)
  {
    return ruleBuilder.Must(BeAValidCountry).WithErrorCode("CountryValidator")
      .WithMessage(x => $"'{{PropertyName}}' must be one of the following: {string.Join(", ", PostalAddressHelper.SupportedCountries)}");
  }
  private static bool BeAValidCountry(string? country) => country == null
    || PostalAddressHelper.GetCountry(country) != null;

  public static IRuleBuilder<T, string?> Identifier<T>(this IRuleBuilder<T, string?> ruleBuilder)
  {
    return ruleBuilder.Must(BeAValidIdentifier).WithErrorCode("IdentifierValidator")
      .WithMessage("'{PropertyName}' may only contain letters, digits and underscores, and may not start with a digit.");
  }
  private static bool BeAValidIdentifier(string? identifier) => identifier == null
    || (!string.IsNullOrEmpty(identifier) && !char.IsDigit(identifier.First()) && identifier.All(c => char.IsLetterOrDigit(c) || c == '_'));

  public static IRuleBuilder<T, CultureInfo?> Locale<T>(this IRuleBuilder<T, CultureInfo?> ruleBuilder)
  {
    return ruleBuilder.Must(BeAValidLocale).WithErrorCode("LocaleValidator")
      .WithMessage("'{PropertyName}' must be an instance of the CultureInfo class with a non-empty name and a LCID different from 4096.");
  }
  private static bool BeAValidLocale(CultureInfo? locale) => locale == null
    || (locale.LCID != 4096 && !string.IsNullOrWhiteSpace(locale.Name));

  public static IRuleBuilder<T, string?> NullOrNotEmpty<T>(this IRuleBuilder<T, string?> ruleBuilder)
  {
    return ruleBuilder.Must(BeNullOrNotEmpty).WithErrorCode("NullOrNotEmptyValidator")
      .WithMessage("'{PropertyName}' must be null or a non-empty string.");
  }
  private static bool BeNullOrNotEmpty(string? s) => s == null || !string.IsNullOrWhiteSpace(s);

  public static IRuleBuilder<T, DateTime?> Past<T>(this IRuleBuilder<T, DateTime?> ruleBuilder, DateTime? moment = null)
  {
    return ruleBuilder.Must(value => BeInThePast(value, moment ?? DateTime.UtcNow))
      .WithErrorCode("PastValidator")
      .WithMessage("'{PropertyName}' must be in the past.");
  }
  private static bool BeInThePast(DateTime? value, DateTime moment) => value == null || value < moment;

  public static IRuleBuilder<T, IPhoneNumber?> PhoneNumber<T>(this IRuleBuilder<T, IPhoneNumber> ruleBuilder)
  {
    return ruleBuilder.Must(p => p.IsValid()).WithErrorCode("PhoneNumberValidator")
      .WithMessage("The phone number is not valid.");
  }

  public static IRuleBuilder<T, string?> Purpose<T>(this IRuleBuilder<T, string?> ruleBuilder)
  {
    return ruleBuilder.Must(BeAValidPurpose).WithErrorCode("PurposeValidator")
      .WithMessage("'{PropertyName}' must be composed of non-empty letter-only words, separated by underscores '_'.");
  }
  private static bool BeAValidPurpose(string? purpose) => purpose == null
    || purpose.Split('_').All(w => !string.IsNullOrEmpty(w) && w.All(char.IsLetter));

  public static IRuleBuilder<T, string?> Username<T>(this IRuleBuilder<T, string?> ruleBuilder, IUsernameSettings usernameSettings)
  {
    return ruleBuilder.Must(u => BeAValidUsername(u, usernameSettings))
      .WithErrorCode("UsernameValidator")
      .WithMessage($"'{{PropertyName}}' may only contain the following characters: {usernameSettings.AllowedCharacters}");
  }
  private static bool BeAValidUsername(string? username, IUsernameSettings usernameSettings)
    => username == null || usernameSettings.AllowedCharacters == null || username.All(usernameSettings.AllowedCharacters.Contains);
}
