using FluentValidation;

namespace Logitar.Portal.Domain.Realms.Validators;

internal class ReadOnlyClaimMappingValidator : AbstractValidator<ReadOnlyClaimMapping>
{
  public ReadOnlyClaimMappingValidator()
  {
    RuleFor(x => x.Name).NotEmpty()
      .MaximumLength(byte.MaxValue);

    When(x => x.Type != null, () => RuleFor(x => x.Type).NotEmpty()
      .MaximumLength(byte.MaxValue));
  }
}
