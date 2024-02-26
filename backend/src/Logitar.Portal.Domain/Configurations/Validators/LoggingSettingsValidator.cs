using FluentValidation;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Domain.Configurations.Validators;

public class LoggingSettingsValidator : AbstractValidator<ILoggingSettings>
{
  public LoggingSettingsValidator()
  {
    RuleFor(x => x.Extent).IsInEnum();
    When(x => x.Extent == LoggingExtent.None, () => RuleFor(x => x.OnlyErrors).Equal(false));
  }
}
