using FluentValidation;
using Logitar.Portal.Domain.Configurations;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Configurations
{
  internal class ConfigurationValidator : AbstractValidator<Configuration>
  {
    public ConfigurationValidator(IValidator<PasswordSettings> passwordSettingsValidator, IValidator<UsernameSettings> usernameSettingsValidator)
    {
      RuleFor(x => x.DefaultLocale).Locale();

      RuleFor(x => x.JwtSecret).NotEmpty()
        .MinimumLength(256 / 8);

      RuleFor(x => x.UsernameSettings).SetValidator(usernameSettingsValidator);

      RuleFor(x => x.PasswordSettings).SetValidator(passwordSettingsValidator);
    }
  }
}
