using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class DictionaryEntryContractValidator : AbstractValidator<DictionaryEntry>
{
  public DictionaryEntryContractValidator()
  {
    RuleFor(x => x.Key).SetValidator(new IdentifierValidator());
    RuleFor(x => x.Value).NotEmpty();
  }
}
