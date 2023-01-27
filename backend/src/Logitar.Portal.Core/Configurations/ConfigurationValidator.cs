using FluentValidation;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Configurations
{
  internal class ConfigurationValidator : AbstractValidator<Configuration>
  {
    public ConfigurationValidator(IValidator<PasswordSettings> passwordSettingsValidator,
      IValidator<UsernameSettings> usernameSettingsValidator)
    {
      RuleFor(x => x.DefaultLocale)
        .NotNull()
        .Locale();

      RuleFor(x => x.JwtSecret)
        .NotEmpty()
        .MinimumLength(256 / 8);

      RuleFor(x => x.UsernameSettings)
        .NotNull()
        .SetValidator(usernameSettingsValidator);

      RuleFor(x => x.PasswordSettings)
        .NotNull()
        .SetValidator(passwordSettingsValidator);
    }
  }
}
