using FluentValidation;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Domain.Configurations;

public record ReadOnlyLoggingSettings : ILoggingSettings
{
  public ReadOnlyLoggingSettings(LoggingExtent extent = LoggingExtent.ActivityOnly, bool onlyErrors = false)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;

    new ReadOnlyLoggingSettingsValidator().ValidateAndThrow(this);
  }

  public LoggingExtent Extent { get; }
  public bool OnlyErrors { get; }
}

internal class ReadOnlyLoggingSettingsValidator : AbstractValidator<ReadOnlyLoggingSettings>
{
  public ReadOnlyLoggingSettingsValidator()
  {
    RuleFor(x => x.Extent).IsInEnum();

    When(x => x.Extent == LoggingExtent.None, () => RuleFor(x => x.OnlyErrors).Equal(false));
  }
}
