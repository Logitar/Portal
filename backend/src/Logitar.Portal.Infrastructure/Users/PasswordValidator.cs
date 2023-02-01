using FluentValidation;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class PasswordValidator : AbstractValidator<string>
  {
    /// <summary>
    /// TODO(fpion): WithErrorCode?
    /// TODO(fpion): WithMessage?
    /// </summary>
    /// <param name="passwordSettings"></param>
    public PasswordValidator(PasswordSettings passwordSettings)
    {
      RuleFor(x => x).NotEmpty();

      if (passwordSettings.RequiredLength > 1)
      {
        RuleFor(x => x)
          .MinimumLength(passwordSettings.RequiredLength);
      }

      if (passwordSettings.RequiredUniqueChars > 1)
      {
        RuleFor(x => x)
          .Must(x => x.GroupBy(c => c).Count() >= passwordSettings.RequiredUniqueChars);
      }

      if (passwordSettings.RequireNonAlphanumeric)
      {
        RuleFor(x => x)
          .Must(x => x.Any(c => !char.IsLetterOrDigit(c)));
      }

      if (passwordSettings.RequireLowercase)
      {
        RuleFor(x => x)
          .Must(x => x.Any(char.IsLower));
      }

      if (passwordSettings.RequireUppercase)
      {
        RuleFor(x => x)
          .Must(x => x.Any(char.IsUpper));
      }

      if (passwordSettings.RequireDigit)
      {
        RuleFor(x => x)
          .Must(x => x.Any(char.IsDigit));
      }
    }
  }
}
