using FluentValidation;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Configurations.Validators;

namespace Logitar.Portal.Domain.Configurations;

public record ReadOnlyLoggingSettings : ILoggingSettings
{
  public LoggingExtent Extent { get; }
  public bool OnlyErrors { get; }

  public ReadOnlyLoggingSettings() : this(LoggingExtent.ActivityOnly, onlyErrors: false)
  {
  }

  public ReadOnlyLoggingSettings(ILoggingSettings logging) : this(logging.Extent, logging.OnlyErrors)
  {
  }

  public ReadOnlyLoggingSettings(LoggingExtent extent, bool onlyErrors)
  {
    Extent = extent;
    OnlyErrors = onlyErrors;
    new LoggingSettingsValidator().ValidateAndThrow(this);
  }
}
