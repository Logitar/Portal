using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal class IdentifierKeyValidator : AbstractValidator<string>
{
  public IdentifierKeyValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier()
      .WithPropertyName(propertyName);
  }
}
