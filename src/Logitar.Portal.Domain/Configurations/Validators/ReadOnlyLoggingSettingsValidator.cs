using FluentValidation;

namespace Logitar.Portal.Domain.Configurations.Validators;

internal class ReadOnlyLoggingSettingsValidator : AbstractValidator<ReadOnlyLoggingSettings>
{
  public ReadOnlyLoggingSettingsValidator()
  {
    RuleFor(x => x.Extent).IsInEnum();

    When(x => x.Extent == LoggingExtent.None, () => RuleFor(x => x.OnlyErrors).Equal(false));
  }
}
