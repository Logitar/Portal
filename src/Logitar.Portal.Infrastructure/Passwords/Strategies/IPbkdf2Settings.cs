using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace Logitar.Portal.Infrastructure.Passwords.Strategies;

public interface IPbkdf2Settings
{
  KeyDerivationPrf Algorithm { get; }
  int Iterations { get; }
  int SaltLength { get; }
  int? HashLength { get; }
}
