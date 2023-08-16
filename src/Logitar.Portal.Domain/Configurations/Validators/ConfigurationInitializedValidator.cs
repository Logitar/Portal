using FluentValidation;
using Logitar.Identity.Domain.Validators;
using Logitar.Portal.Domain.Configurations.Events;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Configurations.Validators;

internal class ConfigurationInitializedValidator : AbstractValidator<ConfigurationInitializedEvent>
{
  public ConfigurationInitializedValidator()
  {
    RuleFor(x => x.DefaultLocale).NotNull()
      .Locale();

    RuleFor(x => x.Secret).NotNull()
      .SetValidator(new SecretValidator());

    RuleFor(x => x.UniqueNameSettings).NotNull()
      .SetValidator(new ReadOnlyUniqueNameSettingsValidator());

    RuleFor(x => x.PasswordSettings).NotNull()
      .SetValidator(new ReadOnlyPasswordSettingsValidator());

    RuleFor(x => x.LoggingSettings).NotNull()
      .SetValidator(new ReadOnlyLoggingSettingsValidator());
  }
}
