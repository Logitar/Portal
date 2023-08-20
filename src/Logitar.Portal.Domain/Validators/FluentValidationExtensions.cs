using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

public static class FluentValidationExtensions
{
  public static IRuleBuilderOptions<T, string> Slug<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.Must(BeAValidSlug)
      .WithErrorCode("")
      .WithMessage("'{PropertyName}' must be composed of non-empty alphanumeric words separated by hyphens (-).");
  }
  private static bool BeAValidSlug(string slug)
    => slug.Split('-').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit));

  public static IRuleBuilderOptions<T, TProperty> WithPropertyName<T, TProperty>(this IRuleBuilderOptions<T, TProperty> options, string? propertyName)
  {
    return propertyName == null ? options : options.WithPropertyName(propertyName);
  }
}
