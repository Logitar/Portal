using FluentValidation;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  internal class UsernameSettingsValidator : AbstractValidator<UsernameSettings>
  {
    public UsernameSettingsValidator()
    {
      RuleFor(x => x.AllowedCharacters).NullOrNotEmpty();
    }
  }
}
