using FluentValidation;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Templates.Validators;

public class UniqueKeyValidator : AbstractValidator<string>
{
  public UniqueKeyValidator(string? propertyName = null)
  {
    RuleFor(x => x).SetValidator(new IdentifierValidator()).WithPropertyName(propertyName);
  }
}
