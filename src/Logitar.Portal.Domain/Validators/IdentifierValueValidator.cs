using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal class IdentifierValueValidator : AbstractValidator<string>
{
  public IdentifierValueValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .WithPropertyName(propertyName);
  }
}
