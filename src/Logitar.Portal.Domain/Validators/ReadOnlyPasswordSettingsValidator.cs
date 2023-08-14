using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal class ReadOnlyPasswordSettingsValidator : AbstractValidator<ReadOnlyPasswordSettings>
{
  public ReadOnlyPasswordSettingsValidator()
  {
    RuleFor(x => x.RequiredLength).GreaterThanOrEqualTo(0);

    RuleFor(x => x.RequiredUniqueChars).GreaterThanOrEqualTo(0)
      .LessThanOrEqualTo(x => x.RequiredLength);

    RuleFor(x => x.Strategy).NotEmpty();
  }
}
