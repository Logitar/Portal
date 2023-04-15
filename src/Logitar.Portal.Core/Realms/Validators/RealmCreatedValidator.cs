using FluentValidation;
using Logitar.Portal.Core.Realms.Events;

namespace Logitar.Portal.Core.Realms.Validators;

internal class RealmCreatedValidator : RealmSavedValidator<RealmCreated>
{
  public RealmCreatedValidator() : base()
  {
    RuleFor(x => x.UniqueName).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Alias();
  }
}
