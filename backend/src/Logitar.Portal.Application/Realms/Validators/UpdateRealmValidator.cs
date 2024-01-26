using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Application.Validators;
using Logitar.Portal.Contracts.Realms;
using Logitar.Portal.Domain.Realms.Validators;
using Logitar.Portal.Domain.Settings.Validators;

namespace Logitar.Portal.Application.Realms.Validators;

internal class UpdateRealmValidator : AbstractValidator<UpdateRealmPayload>
{
  public UpdateRealmValidator()
  {
    When(x => x.UniqueSlug != null, () => RuleFor(x => x.UniqueSlug!).SetValidator(new UniqueSlugValidator()));
    When(x => x.DisplayName?.Value != null, () => RuleFor(x => x.DisplayName!.Value!).SetValidator(new DisplayNameValidator()));
    When(x => x.Description?.Value != null, () => RuleFor(x => x.Description!.Value!).SetValidator(new DescriptionValidator()));

    When(x => x.DefaultLocale?.Value != null, () => RuleFor(x => x.DefaultLocale!.Value!).SetValidator(new LocaleValidator()));
    When(x => x.Secret?.Value != null, () => RuleFor(x => x.Secret!.Value!).SetValidator(new JwtSecretValidator()));
    When(x => x.Url?.Value != null, () => RuleFor(x => x.Url!.Value!).SetValidator(new UrlValidator()));

    When(x => x.UniqueNameSettings != null, () => RuleFor(x => x.UniqueNameSettings!).SetValidator(new UniqueNameSettingsValidator()));
    When(x => x.PasswordSettings != null, () => RuleFor(x => x.PasswordSettings!).SetValidator(new PasswordSettingsValidator()));

    RuleForEach(x => x.CustomAttributes).SetValidator(new CustomAttributeModificationValidator());
  }
}
