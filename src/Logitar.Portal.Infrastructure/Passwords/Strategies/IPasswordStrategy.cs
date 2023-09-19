using Logitar.Portal.Domain.Passwords;

namespace Logitar.Portal.Infrastructure.Passwords.Strategies;

public interface IPasswordStrategy
{
  string Key { get; }

  Password Create(string password);
  Password Decode(string encoded);
}
