using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations.Validators;
using Logitar.Portal.Domain.Settings.Validators;

namespace Logitar.Portal.Application.Configurations.Validators;

internal class ReplaceConfigurationValidator : AbstractValidator<ReplaceConfigurationPayload>
{
  public ReplaceConfigurationValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.DefaultLocale), () => RuleFor(x => x.DefaultLocale!).SetValidator(new LocaleValidator()));

    RuleFor(x => x.UniqueNameSettings).SetValidator(new UniqueNameSettingsValidator());
    RuleFor(x => x.PasswordSettings).SetValidator(new PasswordSettingsValidator());

    RuleFor(x => x.LoggingSettings).SetValidator(new LoggingSettingsValidator());
  }
}
