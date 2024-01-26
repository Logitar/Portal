using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms.Validators;
using Logitar.Portal.Domain.Settings.Validators;

namespace Logitar.Portal.Application.Realms.Validators;

internal class CreateRealmValidator : AbstractValidator<CreateRealmPayload>
{
  public CreateRealmValidator()
  {
    When(x => x.Id != null, () => RuleFor(x => x.Id!).SetValidator(new IdentifierValidator()));

    RuleFor(x => x.UniqueSlug).SetValidator(new UniqueSlugValidator());
    When(x => x.DisplayName != null, () => RuleFor(x => x.DisplayName!).SetValidator(new DisplayNameValidator()));
    When(x => x.Description != null, () => RuleFor(x => x.Description!).SetValidator(new DescriptionValidator()));

    When(x => x.DefaultLocale != null, () => RuleFor(x => x.DefaultLocale!).SetValidator(new LocaleValidator()));
    When(x => x.Secret != null, () => RuleFor(x => x.Secret!).SetValidator(new JwtSecretValidator()));
    When(x => x.Url != null, () => RuleFor(x => x.Url!).SetValidator(new UrlValidator()));

    RuleFor(x => x.UniqueNameSettings).SetValidator(new UniqueNameSettingsValidator());
    RuleFor(x => x.PasswordSettings).SetValidator(new PasswordSettingsValidator());

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeContractValidator());
  }
}
