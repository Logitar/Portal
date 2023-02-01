using FluentValidation;
using Logitar.Portal.Application.Users;
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
