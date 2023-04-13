using FluentValidation;
using Logitar.Portal.Core.Dictionaries.Events;

namespace Logitar.Portal.Core.Dictionaries.Validators;

internal class DictionaryCreatedValidator : DictionarySavedValidator<DictionaryCreated>
{
  public DictionaryCreatedValidator() : base()
  {
    RuleFor(x => x.Locale).NotNull()
      .Locale();
  }
}
