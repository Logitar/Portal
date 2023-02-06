using FluentValidation;
using Logitar.Portal.Domain.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries
{
  internal class DictionaryValidator : AbstractValidator<Dictionary>
  {
    public DictionaryValidator()
    {
      RuleFor(x => x.Locale).Locale();

      RuleForEach(x => x.Entries.Keys).NotEmpty()
        .MaximumLength(255)
        .Identifier();
      RuleForEach(x => x.Entries.Values).NotNull();
    }
  }
}
