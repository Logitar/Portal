using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Dictionaries.Validators;

internal class DictionaryEntryValueValidator : AbstractValidator<string>
{
  public DictionaryEntryValueValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .WithPropertyName(propertyName);
  }
}
