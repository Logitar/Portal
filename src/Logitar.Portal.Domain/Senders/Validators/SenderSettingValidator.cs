using FluentValidation;

namespace Logitar.Portal.Domain.Senders.Validators;

internal class SenderSettingValidator
{
  public SenderSettingValidator(IValidator<string>? keyValidator = null, IValidator<string>? valueValidator = null)
  {
    KeyValidator = keyValidator ?? new SenderSettingKeyValidator("Key");
    ValueValidator = valueValidator ?? new SenderSettingValueValidator("Value");
  }

  public IValidator<string> KeyValidator { get; }
  public IValidator<string> ValueValidator { get; }

  public void ValidateAndThrow(string key, string value)
  {
    KeyValidator.ValidateAndThrow(key);
    ValueValidator.ValidateAndThrow(value);
  }
}
