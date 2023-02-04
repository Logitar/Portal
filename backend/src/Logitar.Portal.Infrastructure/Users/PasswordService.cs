using FluentValidation;
using Logitar.Portal.Application.Users;
using Logitar.Portal.Domain.ApiKeys;
using Logitar.Portal.Domain.Sessions;
using Logitar.Portal.Domain.Users;
using System.Security.Cryptography;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class PasswordService : IPasswordService
  {
    public string GenerateAndHash(int length, out byte[] bytes)
    {
      bytes = RandomNumberGenerator.GetBytes(length);

      return Hash(Convert.ToBase64String(bytes));
    }

    public string Hash(string password)
    {
      Pbkdf2 pbkdf2 = new(password);

      return pbkdf2.ToString();
    }

    public bool IsMatch(ApiKey apiKey, byte[] secret)
    {
      return Pbkdf2.Parse(apiKey.SecretHash).IsMatch(Convert.ToBase64String(secret));
    }

    public bool IsMatch(Session session, byte[] key)
    {
      return session.IsPersistent && Pbkdf2.Parse(session.KeyHash!).IsMatch(Convert.ToBase64String(key));
    }

    public bool IsMatch(User user, string password)
    {
      return user.HasPassword && Pbkdf2.Parse(user.PasswordHash!).IsMatch(password);
    }

    public void ValidateAndThrow(string password, PasswordSettings passwordSettings)
    {
      PasswordValidator validator = new(passwordSettings);
      validator.ValidateAndThrow(password);
    }
  }
}
