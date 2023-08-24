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

  public static IRuleBuilderOptions<T, TProperty> WithPropertyName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> options, string? propertyName)
  {
    return propertyName == null ? options : options.WithName(propertyName);
  }

  private static string BuildErrorCode(string name) => $"{name}Validator";
}
