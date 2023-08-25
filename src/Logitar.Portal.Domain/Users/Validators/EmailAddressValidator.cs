using FluentValidation;

namespace Logitar.Portal.Domain.Users.Validators;

internal class EmailAddressValidator : AbstractValidator<EmailAddress>
{
  public EmailAddressValidator()
  {
    RuleFor(x => x.Address).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .NotEmpty();
  }
}
