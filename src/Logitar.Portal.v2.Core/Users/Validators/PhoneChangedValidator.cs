using FluentValidation;
using Logitar.Portal.v2.Core.Users.Events;

namespace Logitar.Portal.v2.Core.Users.Validators;

internal class PhoneChangedValidator : AbstractValidator<PhoneChanged>
{
  public PhoneChangedValidator()
  {
    When(x => x.Phone == null, () => RuleFor(x => x.VerificationAction).NotEqual(VerificationAction.Verify))
      .Otherwise(() => RuleFor(x => x.Phone!).SetValidator(new ReadOnlyPhoneValidator()));
  }
}
