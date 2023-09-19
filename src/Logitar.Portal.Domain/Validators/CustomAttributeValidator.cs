using FluentValidation;

namespace Logitar.Portal.Domain.Validators;

internal class CustomAttributeValidator
{
  public CustomAttributeValidator(IValidator<string>? keyValidator = null, IValidator<string>? valueValidator = null)
  {
    KeyValidator = keyValidator ?? new CustomAttributeKeyValidator("Key");
    ValueValidator = valueValidator ?? new CustomAttributeValueValidator("Value");
  }

  public IValidator<string> KeyValidator { get; }
  public IValidator<string> ValueValidator { get; }

  public void ValidateAndThrow(string key, string value)
  {
    KeyValidator.ValidateAndThrow(key);
    ValueValidator.ValidateAndThrow(value);
  }
}
