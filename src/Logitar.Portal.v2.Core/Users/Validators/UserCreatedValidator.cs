using FluentValidation;
using Logitar.Portal.v2.Core.Realms;
using Logitar.Portal.v2.Core.Users.Events;

namespace Logitar.Portal.v2.Core.Users.Validators;

internal class UserCreatedValidator : UserSavedValidator<UserCreated>
{
  public UserCreatedValidator(IUsernameSettings usernameSettings) : base()
  {
    RuleFor(x => x.Username).NotEmpty()
      .MaximumLength(byte.MaxValue)
      .Username(usernameSettings);
  }
}
