using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Users.Validators;

internal class BirthdateValidator : AbstractValidator<DateTime>
{
  public BirthdateValidator(string? propertyName = null)
  {
    RuleFor(x => x).Past()
      .WithPropertyName(propertyName);
  }
}
