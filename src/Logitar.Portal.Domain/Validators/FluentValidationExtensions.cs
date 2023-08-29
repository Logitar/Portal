using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal static class FluentValidationExtensions
{
  public static IRuleBuilderOptions<T, string> AllowedCharacters<T>(this IRuleBuilder<T, string> ruleBuilder, string? allowedCharacters)
  {
    return ruleBuilder.Must(value => OnlyContainAllowedCharacters(value, allowedCharacters))
      .WithErrorCode(BuildErrorCode(nameof(AllowedCharacters)))
      .WithMessage($"'{{PropertyName}}' may only contain the following characters: {allowedCharacters}");
  }
  private static bool OnlyContainAllowedCharacters(string value, string? allowedCharacters)
  {
    return allowedCharacters == null || value.All(allowedCharacters.Contains);
  }

  public static IRuleBuilderOptions<T, DateTime> Future<T>(this IRuleBuilder<T, DateTime> ruleBuilder, DateTime? moment = null)
  {
    return ruleBuilder.Must(value => BeInTheFuture(value, moment))
      .WithErrorCode(BuildErrorCode(nameof(Future)))
      .WithMessage("'{PropertyName}' must be a date and time set in the future.");
  }
  private static bool BeInTheFuture(DateTime value, DateTime? moment = null)
  {
    return value > (moment ?? DateTime.Now);
  }

  public static IRuleBuilderOptions<T, string> Identifier<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.Must(BeAValidIdentifier)
      .WithErrorCode(BuildErrorCode(nameof(Identifier)))
      .WithMessage("'{PropertyName}' may not start with a digit, and it may only contains letters, digits and underscores (_).");
  }
  private static bool BeAValidIdentifier(string identifier)
  {
    return !string.IsNullOrEmpty(identifier) && !char.IsDigit(identifier.First()) && identifier.All(c => char.IsLetterOrDigit(c) || c == '_');
  }

  public static IRuleBuilderOptions<T, DateTime> Past<T>(this IRuleBuilder<T, DateTime> ruleBuilder, DateTime? moment = null)
  {
    return ruleBuilder.Must(value => BeInThePast(value, moment))
      .WithErrorCode(BuildErrorCode(nameof(Past)))
      .WithMessage("'{PropertyName}' must be a date and time set in the past.");
  }
  private static bool BeInThePast(DateTime value, DateTime? moment = null)
  {
    return value <= (moment ?? DateTime.Now);
  }

  public static IRuleBuilderOptions<T, string> Slug<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.Must(BeAValidSlug)
      .WithErrorCode(BuildErrorCode(nameof(Slug)))
      .WithMessage("'{PropertyName}' must be composed of non-empty alphanumeric words separated by hyphens (-).");
  }
  private static bool BeAValidSlug(string slug)
  {
    return slug.Split('-').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit));
  }

  public static IRuleBuilderOptions<T, TProperty> WithPropertyName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> options, string? propertyName)
  {
    return propertyName == null ? options : options.WithName(propertyName);
  }

  private static string BuildErrorCode(string name) => $"{name}Validator";
}
