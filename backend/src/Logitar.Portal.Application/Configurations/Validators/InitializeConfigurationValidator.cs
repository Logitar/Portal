using FluentValidation;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Configurations;

namespace Logitar.Portal.Application.Configurations.Validators;

internal class InitializeConfigurationValidator : AbstractValidator<InitializeConfigurationPayload>
{
  public InitializeConfigurationValidator()
  {
    When(x => !string.IsNullOrWhiteSpace(x.DefaultLocale), () => RuleFor(x => x.DefaultLocale!).SetValidator(new LocaleValidator()));
    RuleFor(x => x.Session).SetValidator(new SessionPayloadValidator());
  }
}
