using FluentValidation;

namespace Logitar.Portal.Domain.Dictionaries.Validators;

internal class DictionaryEntryValidator
{
  public DictionaryEntryValidator(IValidator<string>? keyValidator = null, IValidator<string>? valueValidator = null)
  {
    KeyValidator = keyValidator ?? new DictionaryEntryKeyValidator("Key");
    ValueValidator = valueValidator ?? new DictionaryEntryValueValidator("Value");
  }

  public IValidator<string> KeyValidator { get; }
  public IValidator<string> ValueValidator { get; }

  public void ValidateAndThrow(string key, string value)
  {
    KeyValidator.ValidateAndThrow(key);
    ValueValidator.ValidateAndThrow(value);
  }
}
