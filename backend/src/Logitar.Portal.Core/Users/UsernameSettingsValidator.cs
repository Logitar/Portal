using FluentValidation;

namespace Logitar.Portal.Core.Users
{
  internal class UsernameSettingsValidator : AbstractValidator<UsernameSettings>
  {
    public UsernameSettingsValidator()
    {
      RuleFor(x => x.AllowedCharacters).NullOrNotEmpty();
    }
  }
}
