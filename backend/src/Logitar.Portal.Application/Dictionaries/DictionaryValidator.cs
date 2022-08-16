using FluentValidation;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries
{
  internal class DictionaryValidator : AbstractValidator<Dictionary>
  {
    public DictionaryValidator()
    {
      RuleFor(x => x.Locale)
        .Must(ValidationRules.BeAValidCulture);

      RuleForEach(x => x.Entries)
        .SetValidator(new EntryValidator());
    }
  }
}
