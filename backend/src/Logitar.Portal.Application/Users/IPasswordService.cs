using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;

namespace Logitar.Portal.Application.Users
{
  public interface IPasswordService
  {
    string GenerateAndHash(int length, out byte[] bytes);
    string Hash(string password);
    bool IsMatch(Session session, byte[] key);
    bool IsMatch(User user, string password);
    void ValidateAndThrow(string password, PasswordSettings passwordSettings);
  }
}
