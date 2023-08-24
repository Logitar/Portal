using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Users.Validators;

internal class PersonNameValidator : AbstractValidator<string>
{
  public PersonNameValidator(string? propertyName = null)
  {
    When(x => x != null, () => RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .WithPropertyName(propertyName));
  }
}
