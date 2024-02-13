using FluentValidation;
using Logitar.Portal.Contracts.Dictionaries;
using Logitar.Portal.Domain.Dictionaries.Validators;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class DictionaryEntryModificationValidator : AbstractValidator<DictionaryEntryModification>
{
  public DictionaryEntryModificationValidator()
  {
    RuleFor(x => x.Key).SetValidator(new DictionaryEntryKeyValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Value), () => RuleFor(x => x.Value!).SetValidator(new DictionaryEntryValueValidator()));
  }
}
