using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal class IdentifierValidator
{
  public IdentifierValidator(IValidator<string>? keyValidator = null, IValidator<string>? valueValidator = null)
  {
    KeyValidator = keyValidator ?? new IdentifierKeyValidator("Key");
    ValueValidator = valueValidator ?? new IdentifierValueValidator("Value");
  }

  public IValidator<string> KeyValidator { get; }
  public IValidator<string> ValueValidator { get; }

  public void ValidateAndThrow(string key, string value)
  {
    KeyValidator.ValidateAndThrow(key);
    ValueValidator.ValidateAndThrow(value);
  }
}
