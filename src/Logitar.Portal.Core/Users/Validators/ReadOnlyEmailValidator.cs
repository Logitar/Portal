using FluentValidation;
using Logitar.Portal.Core.Users.Contact;

namespace Logitar.Portal.Core.Users.Validators;

internal class ReadOnlyEmailValidator : AbstractValidator<ReadOnlyEmail>
{
  public ReadOnlyEmailValidator()
  {
    RuleFor(x => x.Address).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .EmailAddress();
  }
}
