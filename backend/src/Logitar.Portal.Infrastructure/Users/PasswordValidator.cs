using FluentValidation;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class PasswordValidator : AbstractValidator<string>
  {
    public PasswordValidator(PasswordSettings settings)
    {
      RuleFor(x => x).NotEmpty();

      if (settings.RequiredLength > 1)
      {
        RuleFor(x => x).MinimumLength(settings.RequiredLength)
          .WithErrorCode("PasswordTooShort")
          .WithMessage($"Passwords must be at least {settings.RequiredLength} characters.");
      }

      if (settings.RequiredUniqueChars > 1)
      {
        RuleFor(x => x).Must(x => x.GroupBy(c => c).Count() >= settings.RequiredUniqueChars)
          .WithErrorCode("PasswordRequiresUniqueChars")
          .WithMessage($"Passwords must use at least {settings.RequiredUniqueChars} different characters.");
      }

      if (settings.RequireNonAlphanumeric)
      {
        RuleFor(x => x).Must(x => x.Any(c => !char.IsLetterOrDigit(c)))
          .WithErrorCode("PasswordRequiresNonAlphanumeric")
          .WithMessage("Passwords must have at least one non alphanumeric character.");
      }

      if (settings.RequireLowercase)
      {
        RuleFor(x => x).Must(x => x.Any(char.IsLower))
          .WithErrorCode("PasswordRequiresLower")
          .WithMessage("Passwords must have at least one lowercase ('a'-'z').");
      }

      if (settings.RequireUppercase)
      {
        RuleFor(x => x).Must(x => x.Any(char.IsUpper))
          .WithErrorCode("PasswordRequiresUpper")
          .WithMessage("Passwords must have at least one uppercase ('A'-'Z').");
      }

      if (settings.RequireDigit)
      {
        RuleFor(x => x).Must(x => x.Any(char.IsDigit))
          .WithErrorCode("PasswordRequiresDigit")
          .WithMessage("Passwords must have at least one digit ('0'-'9').");
      }
    }
  }
}
