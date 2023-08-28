using FluentValidation;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations.Validators;

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
