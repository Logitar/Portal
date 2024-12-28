using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class CreateDictionaryValidator : AbstractValidator<CreateDictionaryPayload>
{
  public CreateDictionaryValidator()
  {
    When(x => x.Id.HasValue, () => RuleFor(x => x.Id!.Value).NotEmpty());

    RuleFor(x => x.Locale).Locale();
  }
}
