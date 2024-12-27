using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class DictionaryEntryContractValidator : AbstractValidator<DictionaryEntry>
{
  public DictionaryEntryContractValidator()
  {
    RuleFor(x => x.Key).Identifier();
    RuleFor(x => x.Value).NotEmpty();
  }
}
