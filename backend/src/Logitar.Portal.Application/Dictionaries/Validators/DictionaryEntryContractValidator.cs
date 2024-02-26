using FluentValidation;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries.Validators;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class DictionaryEntryContractValidator : AbstractValidator<DictionaryEntry>
{
  public DictionaryEntryContractValidator()
  {
    RuleFor(x => x.Key).SetValidator(new DictionaryEntryKeyValidator());
    RuleFor(x => x.Value).SetValidator(new DictionaryEntryValueValidator());
  }
}
