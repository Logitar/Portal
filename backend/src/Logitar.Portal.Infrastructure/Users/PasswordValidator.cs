using FluentValidation;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class PasswordValidator : AbstractValidator<string>
  {
    public PasswordValidator(PasswordSettings settings)
    {
      IRuleBuilderOptions<string, string> rules = RuleFor(x => x).NotEmpty();

      if (settings.RequiredLength > 1)
      {
        rules.MinimumLength(settings.RequiredLength)
          .WithErrorCode("PasswordTooShort")
          .WithMessage($"Passwords must be at least {settings.RequiredLength} characters.");
      }

      if (settings.RequiredUniqueChars > 1)
      {
        rules.Must(p => p.GroupBy(c => c).Count() >= settings.RequiredUniqueChars)
          .WithErrorCode("PasswordRequiresUniqueChars")
          .WithMessage($"Passwords must use at least {settings.RequiredUniqueChars} different characters.");
      }

      if (settings.RequireNonAlphanumeric)
      {
        rules.Must(p => p.Any(c => !char.IsLetterOrDigit(c)))
          .WithErrorCode("PasswordRequiresNonAlphanumeric")
          .WithMessage("Passwords must have at least one non alphanumeric character.");
      }

      if (settings.RequireLowercase)
      {
        rules.Must(p => p.Any(char.IsLower))
          .WithErrorCode("PasswordRequiresLower")
          .WithMessage("Passwords must have at least one lowercase ('a'-'z').");
      }

      if (settings.RequireUppercase)
      {
        rules.Must(p => p.Any(char.IsUpper))
          .WithErrorCode("PasswordRequiresUpper")
          .WithMessage("Passwords must have at least one uppercase ('A'-'Z').");
      }

      if (settings.RequireDigit)
      {
        rules.Must(p => p.Any(char.IsDigit))
          .WithErrorCode("PasswordRequiresDigit")
          .WithMessage("Passwords must have at least one digit ('0'-'9').");
      }
    }
  }
}
