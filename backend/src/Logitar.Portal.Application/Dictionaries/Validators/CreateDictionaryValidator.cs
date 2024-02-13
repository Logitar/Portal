using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class CreateDictionaryValidator : AbstractValidator<CreateDictionaryPayload>
{
  public CreateDictionaryValidator()
  {
    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());
  }
}
