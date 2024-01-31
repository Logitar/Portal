using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations.Validators;
using Logitar.Portal.Domain.Settings.Validators;

namespace Logitar.Portal.Application.Configurations.Validators;

internal class UpdateConfigurationValidator : AbstractValidator<UpdateConfigurationPayload>
{
  public UpdateConfigurationValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.DefaultLocale?.Value), () => RuleFor(x => x.DefaultLocale!.Value!).SetValidator(new LocaleValidator()));

    When(x => x.UniqueNameSettings != null, () => RuleFor(x => x.UniqueNameSettings!).SetValidator(new UniqueNameSettingsValidator()));
    When(x => x.PasswordSettings != null, () => RuleFor(x => x.PasswordSettings!).SetValidator(new PasswordSettingsValidator()));

    When(x => x.LoggingSettings != null, () => RuleFor(x => x.LoggingSettings!).SetValidator(new LoggingSettingsValidator()));
  }
}
