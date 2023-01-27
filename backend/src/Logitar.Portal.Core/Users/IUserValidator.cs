using Logitar.Portal.Core.Configurations;

namespace Logitar.Portal.Core.Users
{
  public interface IUserValidator
  {
    void ValidateAndThrow(User user, Configuration configuration);
  }
}
