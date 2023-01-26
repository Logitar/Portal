using Logitar.Portal.Core2.Configurations;

namespace Logitar.Portal.Core2.Users
{
  public interface IUserValidator
  {
    void ValidateAndThrow(User user, Configuration configuration);
  }
}
