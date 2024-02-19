using FluentValidation;
using Logitar.Identity.Domain;

namespace Logitar.Portal.Domain.Templates.Validators;

public class SubjectValidator : AbstractValidator<string>
{
  public SubjectValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(SubjectUnit.MaximumLength)
      .WithPropertyName(propertyName);
  }
}
