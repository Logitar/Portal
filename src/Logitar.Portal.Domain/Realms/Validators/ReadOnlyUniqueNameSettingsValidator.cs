using FluentValidation;

namespace Logitar.Portal.Domain.Realms.Validators;

internal class ReadOnlyUniqueNameSettingsValidator : AbstractValidator<ReadOnlyUniqueNameSettings>
{
  public ReadOnlyUniqueNameSettingsValidator()
  {
    When(x => x.AllowedCharacters != null, () => RuleFor(x => x.AllowedCharacters).NotEmpty());
  }
}
