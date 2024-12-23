using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class ReplaceDictionaryValidator : AbstractValidator<ReplaceDictionaryPayload>
{
  public ReplaceDictionaryValidator()
  {
    RuleFor(x => x.Locale).Locale();
    RuleForEach(x => x.Entries).SetValidator(new DictionaryEntryContractValidator());
  }
}
