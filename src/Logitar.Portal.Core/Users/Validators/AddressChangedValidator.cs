using FluentValidation;
using Logitar.Portal.Core.Users.Events;

namespace Logitar.Portal.Core.Users.Validators;

internal class AddressChangedValidator : AbstractValidator<AddressChanged>
{
  public AddressChangedValidator()
  {
    When(x => x.Address == null, () => RuleFor(x => x.VerificationAction).NotEqual(VerificationAction.Verify))
      .Otherwise(() => RuleFor(x => x.Address!).SetValidator(new ReadOnlyAddressValidator()));
  }
}
