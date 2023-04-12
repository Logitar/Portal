using FluentValidation;
using Logitar.Portal.v2.Core.Users.Contact;

namespace Logitar.Portal.v2.Core.Users.Validators;

internal class ReadOnlyEmailValidator : AbstractValidator<ReadOnlyEmail>
{
  public ReadOnlyEmailValidator()
  {
    RuleFor(x => x.Address).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .EmailAddress();
  }
}
