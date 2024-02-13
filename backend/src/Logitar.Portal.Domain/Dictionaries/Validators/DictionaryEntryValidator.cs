using FluentValidation;

namespace Logitar.Portal.Domain.Dictionaries.Validators;

public class DictionaryEntryValidator : IDictionaryEntryValidator
{
  public IValidator<string> KeyValidator { get; }
  public IValidator<string> ValueValidator { get; }

  public DictionaryEntryValidator(IValidator<string>? keyValidator = null, IValidator<string>? valueValidator = null)
  {
    KeyValidator = keyValidator ?? new DictionaryEntryKeyValidator("Key");
    ValueValidator = valueValidator ?? new DictionaryEntryValueValidator("Value");
  }

  public void ValidateAndThrow(string key, string value)
  {
    KeyValidator.ValidateAndThrow(key);
    ValueValidator.ValidateAndThrow(value);
  }
}
