using Logitar.Portal.Core.Configurations;

namespace Logitar.Portal.Core.Users
{
  public interface IPasswordService
  {
    string GenerateAndHash(int length, out byte[] password);
    string Hash(string password);
    bool IsMatch(User user, string password);
    void ValidateAndThrow(string password, Configuration configuration);
  }
}
