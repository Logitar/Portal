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

    RuleFor(x => x.Secret).NotEmpty()
      .MinimumLength(256 / 8)
      .MaximumLength(512 / 8);

    RuleFor(x => x.UniqueNameSettings).NotNull()
      .SetValidator(new ReadOnlyUniqueNameSettingsValidator());

    RuleFor(x => x.PasswordSettings).NotNull()
      .SetValidator(new ReadOnlyPasswordSettingsValidator());

    RuleFor(x => x.LoggingSettings).NotNull()
      .SetValidator(new ReadOnlyLoggingSettingsValidator());
  }
}
