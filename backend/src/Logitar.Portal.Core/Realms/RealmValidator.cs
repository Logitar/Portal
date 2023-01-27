using FluentValidation;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Core.Realms
{
  internal class RealmValidator : AbstractValidator<Realm>
  {
    public RealmValidator(IValidator<PasswordSettings> passwordSettingsValidator, IValidator<UsernameSettings> usernameSettingsValidator)
    {
      RuleFor(x => x.Alias)
        .NotEmpty()
        .MaximumLength(256)
        .Alias();

      RuleFor(x => x.DisplayName)
        .NotEmpty()
        .MaximumLength(256);

      RuleFor(x => x.Description).NullOrNotEmpty();

      RuleFor(x => x.DefaultLocale).Locale();

      RuleFor(x => x.Url)
        .MaximumLength(2048)
        .Uri();

      RuleFor(x => x.UsernameSettings).SetValidator(usernameSettingsValidator);

      RuleFor(x => x.PasswordSettings).SetValidator(passwordSettingsValidator);

      RuleFor(x => x.GoogleClientId)
        .MaximumLength(256)
        .NullOrNotEmpty();
    }
  }
}
