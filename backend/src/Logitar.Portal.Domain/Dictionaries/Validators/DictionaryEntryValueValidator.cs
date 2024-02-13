using FluentValidation;
using Logitar.Identity.Domain;

namespace Logitar.Portal.Domain.Dictionaries.Validators;

public class DictionaryEntryValueValidator : AbstractValidator<string>
{
  public DictionaryEntryValueValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty().WithPropertyName(propertyName);
  }
}
