using FluentValidation;
using Logitar.Portal.Domain.Validators;

namespace Logitar.Portal.Domain.Dictionaries.Validators;

internal class DictionaryEntryKeyValidator : AbstractValidator<string>
{
  public DictionaryEntryKeyValidator(string? propertyName = null)
  {
    RuleFor(x => x).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier()
      .WithPropertyName(propertyName);
  }
}
