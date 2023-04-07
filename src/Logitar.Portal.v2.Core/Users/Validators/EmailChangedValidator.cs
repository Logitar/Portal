using FluentValidation;
using Logitar.Portal.v2.Core.Users.Events;

namespace Logitar.Portal.v2.Core.Users.Validators;

internal class EmailChangedValidator : AbstractValidator<EmailChanged>
{
  public EmailChangedValidator()
  {
    When(x => x.Email == null, () => RuleFor(x => x.VerificationAction).NotEqual(VerificationAction.Verify))
      .Otherwise(() => RuleFor(x => x.Email!).SetValidator(new ReadOnlyEmailValidator()));
  }
}
