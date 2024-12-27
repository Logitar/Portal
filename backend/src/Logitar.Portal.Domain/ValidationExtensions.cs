using FluentValidation;
using Logitar.Identity.Core.Users;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain;

public static class ValidationExtensions
{
  public static IRuleBuilderOptions<T, string> ContentType<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(byte.MaxValue).SetValidator(new ContentTypeValidator<T>());
  }

  public static IRuleBuilderOptions<T, string> EmailAddressInput<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Email.MaximumLength).EmailAddress();
  }

  public static IRuleBuilderOptions<T, string> JwtSecret<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MinimumLength(Settings.JwtSecret.MinimumLength).MaximumLength(Settings.JwtSecret.MaximumLength);
  }

  public static IRuleBuilderOptions<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(byte.MaxValue).SetValidator(new PhoneNumberValidator<T>());
  }

  public static IRuleBuilderOptions<T, string> Slug<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Realms.Slug.MaximumLength).SetValidator(new SlugValidator<T>());
  }

  public static IRuleBuilderOptions<T, string> Subject<T>(this IRuleBuilder<T, string> ruleBuilder)
  {
    return ruleBuilder.NotEmpty().MaximumLength(Templates.Subject.MaximumLength);
  }
}
