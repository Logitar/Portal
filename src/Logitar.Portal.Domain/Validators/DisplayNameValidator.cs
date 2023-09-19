using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal class DisplayNameValidator : AbstractValidator<string>
{
  public DisplayNameValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .WithPropertyName(propertyName);
  }
}
