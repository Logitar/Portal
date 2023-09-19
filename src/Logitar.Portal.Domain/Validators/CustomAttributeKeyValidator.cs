using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal class CustomAttributeKeyValidator : AbstractValidator<string>
{
  public CustomAttributeKeyValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier()
      .WithPropertyName(propertyName);
  }
}
