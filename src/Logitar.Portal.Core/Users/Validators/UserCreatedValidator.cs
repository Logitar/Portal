using FluentValidation;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users.Events;

namespace Logitar.Portal.Core.Users.Validators;

internal class UserCreatedValidator : UserSavedValidator<UserCreated>
{
  public UserCreatedValidator(IUsernameSettings usernameSettings) : base()
  {
    RuleFor(x => x.Username).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Username(usernameSettings);
  }
}
