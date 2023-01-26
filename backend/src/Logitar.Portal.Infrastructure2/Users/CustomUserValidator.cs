using FluentValidation;
using Logitar.Portal.Core2.Configurations;
using Logitar.Portal.Core2.Users;

namespace Logitar.Portal.Infrastructure2.Users
{
  internal class CustomUserValidator : IUserValidator
  {
    public void ValidateAndThrow(User user, Configuration configuration)
    {
      UserValidator validator = new(configuration.UsernameSettings);
      validator.ValidateAndThrow(user);
    }
  }
}
