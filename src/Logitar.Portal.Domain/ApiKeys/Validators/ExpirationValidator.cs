using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.ApiKeys.Validators;

internal class ExpirationValidator : AbstractValidator<DateTime>
{
  public ExpirationValidator(string? propertyName = null)
  {
    RuleFor(x => x).Future()
      .WithPropertyName(propertyName);
  }
}
