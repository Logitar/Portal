using FluentValidation;
using Logitar.Portal.Domain.Configurations;

namespace Logitar.Portal.Application.Configurations
{
  internal class LoggingSettingsValidator : AbstractValidator<LoggingSettings>
  {
    public LoggingSettingsValidator()
    {
      RuleFor(x => x.Extent).IsInEnum();

      When(x => x.Extent == LoggingExtent.None, () => RuleFor(x => x.OnlyErrors).Equal(false));
    }
  }
}
