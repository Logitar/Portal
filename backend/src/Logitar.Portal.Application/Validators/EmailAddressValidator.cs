using FluentValidation;
using Logitar.Identity.Domain;

namespace Logitar.Portal.Application.Validators;

internal class EmailAddressValidator : AbstractValidator<string>
{
  public EmailAddressValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .EmailAddress()
      .WithPropertyName(propertyName);
  }
}
