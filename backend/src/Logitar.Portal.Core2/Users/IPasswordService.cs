using Logitar.Portal.Core2.Configurations;

namespace Logitar.Portal.Core2.Users
{
  public interface IPasswordService
  {
    string Hash(string password);
    void ValidateAndThrow(string password, Configuration configuration);
  }
}
