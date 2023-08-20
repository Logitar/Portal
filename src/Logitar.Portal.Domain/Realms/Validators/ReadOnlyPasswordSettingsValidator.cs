using FluentValidation;

namespace Logitar.Portal.Domain.Realms.Validators;

internal class ReadOnlyPasswordSettingsValidator : AbstractValidator<ReadOnlyPasswordSettings>
{
  public ReadOnlyPasswordSettingsValidator()
  {
    RuleFor(x => x.RequiredLength).GreaterThan(0);

    RuleFor(x => x.RequiredUniqueChars).GreaterThan(0)
      .LessThanOrEqualTo(x => x.RequiredLength);
  }
}
