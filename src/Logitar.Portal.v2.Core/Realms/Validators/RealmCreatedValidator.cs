using FluentValidation;
using Logitar.Portal.v2.Core.Realms.Events;

namespace Logitar.Portal.v2.Core.Realms.Validators;

internal class RealmCreatedValidator : RealmSavedValidator<RealmCreated>
{
  public RealmCreatedValidator() : base()
  {
    RuleFor(x => x.UniqueName).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Alias();
  }
}
