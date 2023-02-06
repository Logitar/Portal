using FluentValidation;
using Logitar.Portal.Domain.Realms;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Realms
{
  internal class RealmValidator : AbstractValidator<Realm>
  {
    public RealmValidator(IValidator<PasswordSettings> passwordSettingsValidator, IValidator<UsernameSettings> usernameSettingsValidator)
    {
      RuleFor(x => x.Alias).NotEmpty()
        .MaximumLength(255)
        .Alias();

      RuleFor(x => x.DisplayName).NullOrNotEmpty()
        .MaximumLength(255);

      RuleFor(x => x.Description).NullOrNotEmpty();

      RuleFor(x => x.DefaultLocale).Locale();

      RuleFor(x => x.JwtSecret).NotEmpty()
        .MinimumLength(256 / 8);

      RuleFor(x => x.Url).NullOrNotEmpty()
        .MaximumLength(2048)
        .Url();

      RuleFor(x => x.UsernameSettings).SetValidator(usernameSettingsValidator);

      RuleFor(x => x.PasswordSettings).SetValidator(passwordSettingsValidator);

      RuleFor(x => x.GoogleClientId).NullOrNotEmpty()
        .MaximumLength(255);
    }
  }
}
