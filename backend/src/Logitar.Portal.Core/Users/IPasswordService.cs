using Logitar.Portal.Core.Configurations;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Sessions;

namespace Logitar.Portal.Core.Users
{
  public interface IPasswordService
  {
    string GenerateAndHash(int length, out byte[] password);
    string Hash(string password);
    bool IsMatch(Session session, byte[] password);
    bool IsMatch(User user, string password);
    void ValidateAndThrow(string password);
    void ValidateAndThrow(string password, Configuration configuration);
    void ValidateAndThrow(string password, Realm realm);
    Task ValidateAndThrowAsync(string password, string? realmId = null, CancellationToken cancellationToken = default);
  }
}
