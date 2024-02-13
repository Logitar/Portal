using FluentValidation;

namespace Logitar.Portal.Domain.Dictionaries.Validators;

public interface IDictionaryEntryValidator
{
  IValidator<string> KeyValidator { get; }
  IValidator<string> ValueValidator { get; }

  void ValidateAndThrow(string key, string value);
}
