using FluentValidation;
using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Users;

namespace Logitar.Portal.Infrastructure.Users
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
