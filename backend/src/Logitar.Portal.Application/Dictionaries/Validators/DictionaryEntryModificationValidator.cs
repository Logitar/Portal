using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class DictionaryEntryModificationValidator : AbstractValidator<DictionaryEntryModification>
{
  public DictionaryEntryModificationValidator()
  {
    RuleFor(x => x.Key).Identifier();
    When(x => !string.IsNullOrWhiteSpace(x.Value), () => RuleFor(x => x.Value!).NotEmpty());
  }
}
