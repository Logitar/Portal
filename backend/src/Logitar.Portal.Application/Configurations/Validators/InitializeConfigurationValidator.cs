using FluentValidation;
using Logitar.Identity.Domain.Settings;
using Logitar.Identity.Domain.Shared;
using Logitar.Portal.Contracts.Configurations;
using Logitar.Portal.Domain.Settings;

namespace Logitar.Portal.Application.Configurations.Validators;

internal class InitializeConfigurationValidator : AbstractValidator<InitializeConfigurationPayload>
{
  public InitializeConfigurationValidator()
  {
    UserSettings userSettings = new()
    {
      UniqueName = new ReadOnlyUniqueNameSettings(),
      Password = new ReadOnlyPasswordSettings(),
      RequireUniqueEmail = true
    };

    RuleFor(x => x.Locale).SetValidator(new LocaleValidator());
    RuleFor(x => x.User).SetValidator(new UserPayloadValidator(userSettings));
    RuleFor(x => x.Session).SetValidator(new SessionPayloadValidator());
  }
}
