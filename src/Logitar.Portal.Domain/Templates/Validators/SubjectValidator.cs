using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Templates.Validators;

internal class SubjectValidator : AbstractValidator<string>
{
  public SubjectValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .WithPropertyName(propertyName);
  }
}
