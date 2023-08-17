using FluentValidation;
using Logitar.Portal.Domain.Realms.Events;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Realms.Validators;

internal class RealmCreatedValidator : AbstractValidator<RealmCreatedEvent>
{
  public RealmCreatedValidator()
  {
    RuleFor(x => x.UniqueSlug).SetValidator(new UniqueSlugValidator());

    RuleFor(x => x.Secret).NotNull()
      .SetValidator(new SecretValidator());

    RuleFor(x => x.UniqueNameSettings).NotNull()
      .SetValidator(new ReadOnlyUniqueNameSettingsValidator());

    RuleFor(x => x.PasswordSettings).NotNull()
      .SetValidator(new ReadOnlyPasswordSettingsValidator());
  }
}
