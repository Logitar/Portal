using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class ReplaceDictionaryValidator : AbstractValidator<ReplaceDictionaryPayload>
{
  public ReplaceDictionaryValidator()
  {
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());
    RuleForEach(x => x.Entries).SetValidator(new DictionaryEntryContractValidator());
  }
}
