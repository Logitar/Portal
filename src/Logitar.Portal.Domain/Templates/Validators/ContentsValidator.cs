using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Templates.Validators;

internal class ContentsValidator : AbstractValidator<string>
{
  public ContentsValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .WithPropertyName(propertyName);
  }
}
