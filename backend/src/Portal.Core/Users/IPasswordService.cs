using Portal.Core.Realms;

namespace Portal.Core.Users
{
  public interface IPasswordService
  {
    string GenerateAndHash(int length, out byte[] password);
    string Hash(string password);
    bool IsMatch(User user, string password);
    bool IsMatch(string hash, byte[] password);
    void ValidateAndThrow(string password, Realm? realm = null);
  }
}
