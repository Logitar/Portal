using FluentValidation;
using Logitar.Portal.v2.Core.Users.Events;

namespace Logitar.Portal.v2.Core.Users.Validators;

internal class AddressChangedValidator : AbstractValidator<AddressChanged>
{
  public AddressChangedValidator()
  {
    When(x => x.Address == null, () => RuleFor(x => x.VerificationAction).NotEqual(VerificationAction.Verify))
      .Otherwise(() => RuleFor(x => x.Address!).SetValidator(new ReadOnlyAddressValidator()));
  }
}
