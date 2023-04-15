using FluentValidation;
using Logitar.Portal.Domain.Realms;

namespace Logitar.Portal.Application.Realms
{
  internal class RealmValidator : AbstractValidator<Realm>
  {
    public RealmValidator()
    {
      RuleFor(x => x.Alias)
        .NotEmpty()
        .MaximumLength(256)
        .Must(BeAValidAlias);

      RuleFor(x => x.Name)
        .NotEmpty()
        .MaximumLength(256);

      RuleFor(x => x.DefaultLocale)
        .Must(ValidationRules.BeAValidCulture);

      RuleFor(x => x.Url)
        .MaximumLength(2048)
        .Must(ValidationRules.BeAValidUrl);

      When(x => x.PasswordSettings != null, () => RuleFor(x => x.PasswordSettings!)
        .SetValidator(new PasswordSettingsValidator()));

      RuleFor(x => x.GoogleClientId)
        .MaximumLength(256);
    }

    private static bool BeAValidAlias(string? value) => value == null
      || value.Split('-').All(word => !string.IsNullOrEmpty(word) && word.All(char.IsLetterOrDigit));
  }
}
