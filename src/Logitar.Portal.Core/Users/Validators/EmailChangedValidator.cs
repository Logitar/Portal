using FluentValidation;
using Logitar.Portal.Core.Users.Events;

namespace Logitar.Portal.Core.Users.Validators;

internal class EmailChangedValidator : AbstractValidator<EmailChanged>
{
  public EmailChangedValidator()
  {
    When(x => x.Email == null, () => RuleFor(x => x.VerificationAction).NotEqual(VerificationAction.Verify))
      .Otherwise(() => RuleFor(x => x.Email!).SetValidator(new ReadOnlyEmailValidator()));
  }
}
