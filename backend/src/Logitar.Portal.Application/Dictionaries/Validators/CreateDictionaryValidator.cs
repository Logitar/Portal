using FluentValidation;
using Logitar.Identity.Core;
using Logitar.Portal.Contracts.Dictionaries;

namespace Logitar.Portal.Application.Dictionaries.Validators;

internal class CreateDictionaryValidator : AbstractValidator<CreateDictionaryPayload>
{
  public CreateDictionaryValidator()
  {
    RuleFor(x => x.Locale).Locale());
  }
}
