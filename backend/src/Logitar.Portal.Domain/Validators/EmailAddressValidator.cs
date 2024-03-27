using FluentValidation;
using Logitar.Identity.Domain;

namespace Logitar.Portal.Domain.Validators;

public class EmailAddressValidator : AbstractValidator<string>
{
  public EmailAddressValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .EmailAddress()
      .WithPropertyName(propertyName);
  }
}
