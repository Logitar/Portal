using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class UpdateDictionaryValidator : AbstractValidator<UpdateDictionaryPayload>
{
  public UpdateDictionaryValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.Locale), () => RuleFor(x => x.Locale!).SetValidator(new LocaleValidator()));
    RuleForEach(x => x.Entries).SetValidator(new DictionaryEntryModificationValidator());
  }
}
