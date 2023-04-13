using FluentValidation;
using Logitar.Portal.Core.Dictionaries.Events;

namespace Logitar.Portal.Core.Dictionaries.Validators;

internal abstract class DictionarySavedValidator<T> : AbstractValidator<T> where T : DictionarySaved
{
  protected DictionarySavedValidator()
  {
    RuleForEach(x => x.Entries.Keys).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Identifier();
    RuleForEach(x => x.Entries.Values).NotEmpty();
  }
}
