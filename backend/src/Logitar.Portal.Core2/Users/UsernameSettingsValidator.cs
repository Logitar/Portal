using FluentValidation;

namespace Logitar.Portal.Core2.Users
{
  internal class UsernameSettingsValidator : AbstractValidator<UsernameSettings>
  {
    public UsernameSettingsValidator()
    {
      RuleFor(x => x.AllowedCharacters).NullOrNotEmpty();
    }
  }
}
