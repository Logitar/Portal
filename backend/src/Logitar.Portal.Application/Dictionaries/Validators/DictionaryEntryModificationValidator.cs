using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class DictionaryEntryModificationValidator : AbstractValidator<DictionaryEntryModification>
{
  public DictionaryEntryModificationValidator()
  {
    RuleFor(x => x.Key).SetValidator(new IdentifierValidator());
    When(x => !string.IsNullOrWhiteSpace(x.Value), () => RuleFor(x => x.Value!).NotEmpty());
  }
}
