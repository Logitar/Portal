using FluentValidation;
using Logitar.Portal.Core.Configurations.Events;
using Logitar.Portal.Core.Realms.Validators;

namespace Logitar.Portal.Core.Configurations.Validators;

internal class ConfigurationInitializedValidator : AbstractValidator<ConfigurationInitialized>
{
  public ConfigurationInitializedValidator()
  {
    RuleFor(x => x.DefaultLocale).Locale();

    RuleFor(x => x.UsernameSettings).SetValidator(new ReadOnlyUsernameSettingsValidator());

    RuleFor(x => x.PasswordSettings).SetValidator(new ReadOnlyPasswordSettingsValidator());

    RuleFor(x => x.LoggingSettings).SetValidator(new ReadOnlyLoggingSettingsValidator());
  }
}
