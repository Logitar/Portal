using FluentValidation;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class CustomUserValidator : IUserValidator
  {
    public void ValidateAndThrow(User user, UsernameSettings usernameSettings)
    {
      UserValidator validator = new(usernameSettings);
      validator.ValidateAndThrow(user);
    }
  }
}
