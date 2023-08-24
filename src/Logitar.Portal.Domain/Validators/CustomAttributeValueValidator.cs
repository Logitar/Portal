using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal class CustomAttributeValueValidator : AbstractValidator<string>
{
  public CustomAttributeValueValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .WithPropertyName(propertyName);
  }
}
