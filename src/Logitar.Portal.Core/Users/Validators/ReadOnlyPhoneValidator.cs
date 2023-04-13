using FluentValidation;
using Logitar.Portal.Core.Users.Contact;

namespace Logitar.Portal.Core.Users.Validators;

internal class ReadOnlyPhoneValidator : AbstractValidator<ReadOnlyPhone>
{
  public ReadOnlyPhoneValidator()
  {
    RuleFor(x => x.CountryCode).NullOrNotEmpty()
      .MaximumLength(16);

    RuleFor(x => x.Number).NotEmpty()
      .MaximumLength(32);

    RuleFor(x => x.Extension).NullOrNotEmpty()
      .MaximumLength(16);

    RuleFor(x => x).PhoneNumber();
  }
}
