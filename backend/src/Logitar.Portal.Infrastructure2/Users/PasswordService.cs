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

    public string GenerateAndHash(int length, out byte[] password)
    {
      password = RandomNumberGenerator.GetBytes(length);

      return Hash(Convert.ToBase64String(password));
    }

    public string Hash(string password)
    {
      Pbkdf2 pbkdf2 = new(password, Algorithm, IterationCount, SaltLength);
      return pbkdf2.ToString();
    }

    public bool IsMatch(User user, string password)
    {
      if (user.PasswordHash == null)
      {
        return false;
      }

      Pbkdf2 pbkdf2 = Pbkdf2.Parse(user.PasswordHash);
      return pbkdf2.IsMatch(password);
    }

    public void ValidateAndThrow(string password, Configuration configuration)
    {
      PasswordValidator validator = new(configuration.PasswordSettings);
      validator.ValidateAndThrow(password);
    }
  }
}
