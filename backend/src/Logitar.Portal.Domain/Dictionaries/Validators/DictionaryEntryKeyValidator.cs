using FluentValidation;
using Logitar.Identity.Domain;
using Logitar.Identity.Domain.Shared;

namespace Logitar.Portal.Domain.Dictionaries.Validators;

public class DictionaryEntryKeyValidator : AbstractValidator<string>
{
  public DictionaryEntryKeyValidator(string? propertyName = null)
  {
    RuleFor(x => x).SetValidator(new IdentifierValidator()).WithPropertyName(propertyName);
  }
}
