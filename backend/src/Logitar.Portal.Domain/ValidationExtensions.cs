using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> JwtSecret<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MinimumLength(Settings.JwtSecret.MinimumLength).MaximumLength(Settings.JwtSecret.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> Slug<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Realms.Slug.MaximumLength).SetValidator(new SlugValidator<T>());
  }
}
