using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class UpdateDictionaryValidator : AbstractValidator<UpdateDictionaryPayload>
{
  public UpdateDictionaryValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).Locale());
    RuleForEach(x => x.Entries).SetValidator(new DictionaryEntryModificationValidator());
  }
}
