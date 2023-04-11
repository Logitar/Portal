using FluentValidation;
using Logitar.Portal.v2.Core.Dictionaries.Events;

namespace Logitar.Portal.v2.Core.Dictionaries.Validators;

internal class DictionaryCreatedValidator : DictionarySavedValidator<DictionaryCreated>
{
  public DictionaryCreatedValidator() : base()
  {
    RuleFor(x => x.Locale).NotNull()
      .Locale();
  }
}
