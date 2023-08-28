using Logitar.Portal.Contracts.Settings;

namespace Logitar.Portal.Domain.Passwords;

public interface IPasswordService
{
  Password Create(IPasswordSettings passwordSettings, string password);
  Password Decode(string encoded);
  Password Generate(IPasswordSettings passwordSettings, int length, out byte[] password);
}
