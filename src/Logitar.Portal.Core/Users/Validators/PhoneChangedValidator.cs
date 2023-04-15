using FluentValidation;
using Logitar.Portal.Core.Users.Events;

namespace Logitar.Portal.Core.Users.Validators;

internal class PhoneChangedValidator : AbstractValidator<PhoneChanged>
{
  public PhoneChangedValidator()
  {
    When(x => x.Phone == null, () => RuleFor(x => x.VerificationAction).NotEqual(VerificationAction.Verify))
      .Otherwise(() => RuleFor(x => x.Phone!).SetValidator(new ReadOnlyPhoneValidator()));
  }
}
