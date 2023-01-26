using FluentValidation;
using Logitar.Portal.Core2.Configurations;
using Logitar.Portal.Core2.Users;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Logitar.Portal.Infrastructure2.Users
{
  internal class PasswordService : IPasswordService
  {
    private const KeyDerivationPrf Algorithm = KeyDerivationPrf.HMACSHA256;
    private const int IterationCount = 100000;
    private const int SaltLength = 256 / 8;

    public string Hash(string password)
    {
      byte[] salt = RandomNumberGenerator.GetBytes(SaltLength);
      byte[] hash = KeyDerivation.Pbkdf2(password, salt, Algorithm, IterationCount, salt.Length);

      return string.Join(':', Algorithm, IterationCount, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
    }

    public void ValidateAndThrow(string password, Configuration configuration)
    {
      PasswordValidator validator = new(configuration.PasswordSettings);
      validator.ValidateAndThrow(password);
    }
  }
}
