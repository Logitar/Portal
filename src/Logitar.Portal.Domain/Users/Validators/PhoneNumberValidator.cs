using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Users.Validators;

internal class PhoneNumberValidator : AbstractValidator<PhoneNumber>
{
  public PhoneNumberValidator()
  {
    When(x => x.CountryCode != null, () => RuleFor(x => x.CountryCode).NotEmpty()
      .MaximumLength(10));

    RuleFor(x => x.Number).NotEmpty()
      .MaximumLength(20);

    When(x => x.Extension != null, () => RuleFor(x => x.Extension).NotEmpty()
      .MaximumLength(10));

    RuleFor(x => x).PhoneNumber();
  }
}
