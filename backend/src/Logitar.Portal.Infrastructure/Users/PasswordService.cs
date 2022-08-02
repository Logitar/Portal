using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Logitar.Portal.Core;
using Logitar.Portal.Core.Realms;
using Logitar.Portal.Core.Users;
using System.Security.Cryptography;

namespace Logitar.Portal.Infrastructure.Users
{
  internal class PasswordService : IDisposable, IPasswordService
  {
    private const KeyDerivationPrf Algorithm = KeyDerivationPrf.HMACSHA256;
    private const int IterationCount = 100000;
    private const int SaltLength = 256;

    private readonly RandomNumberGenerator _generator = RandomNumberGenerator.Create();

    private readonly PasswordSettings _settings = new(requiredLength: 8, requiredUniqueChars: 8,
      requireNonAlphanumeric: true, requireLowercase: true, requireUppercase: true, requireDigit: true);

    public void Dispose()
    {
      _generator.Dispose();
    }

    public string GenerateAndHash(int length, out byte[] password)
    {
      password = new byte[length];
      _generator.GetNonZeroBytes(password);

      return Hash(Convert.ToBase64String(password));
    }

    public string Hash(string passwordString)
    {
      ArgumentNullException.ThrowIfNull(passwordString);

      var salt = new byte[SaltLength / 8];
      _generator.GetNonZeroBytes(salt);

      var password = new Password(passwordString, salt, IterationCount, Algorithm);

      return password.ToString();
    }

    public bool IsMatch(User user, string passwordString)
    {
      ArgumentNullException.ThrowIfNull(user);
      ArgumentNullException.ThrowIfNull(passwordString);

      return user.PasswordHash != null && IsMatch(user.PasswordHash, passwordString);
    }
    public bool IsMatch(string hash, byte[] passwordBytes)
    {
      ArgumentNullException.ThrowIfNull(hash);
      ArgumentNullException.ThrowIfNull(passwordBytes);

      return IsMatch(hash, Convert.ToBase64String(passwordBytes));
    }
    private static bool IsMatch(string hash, string passwordString)
    {
      ArgumentNullException.ThrowIfNull(hash);
      ArgumentNullException.ThrowIfNull(passwordString);

      return Password.TryParse(hash, out Password? password)
        && password?.IsMatch(passwordString) == true;
    }

    public void ValidateAndThrow(string passwordString, Realm? realm)
    {
      ArgumentNullException.ThrowIfNull(passwordString);

      PasswordSettings settings;
      if (realm == null)
      {
        settings = _settings;
      }
      else if (realm.PasswordSettings == null)
      {
        return;
      }
      else
      {
        settings = realm.PasswordSettings;
      }

      var errors = new List<Error>(capacity: 6);
      if (passwordString.Length < settings.RequiredLength)
      {
        errors.Add(new Error("PasswordTooShort", $"Passwords must be at least {settings.RequiredLength} characters."));
      }

      var chars = new Dictionary<char, int>(capacity: passwordString.Length);
      foreach (char current in passwordString)
      {
        if (chars.ContainsKey(current))
        {
          chars[current]++;
        }
        else
        {
          chars.Add(current, 1);
        }
      }

      if (chars.Count < settings.RequiredUniqueChars)
      {
        errors.Add(new Error("PasswordRequiresUniqueChars", $"Passwords must use at least {settings.RequiredUniqueChars} different characters."));
      }

      if (settings.RequireNonAlphanumeric && chars.Keys.All(char.IsLetterOrDigit))
        errors.Add(new Error("PasswordRequiresNonAlphanumeric", "Passwords must have at least one non alphanumeric character."));
      if (settings.RequireLowercase && !chars.Keys.Any(char.IsLower))
        errors.Add(new Error("PasswordRequiresLower", "Passwords must have at least one lowercase ('a'-'z')."));
      if (settings.RequireUppercase && !chars.Keys.Any(char.IsUpper))
        errors.Add(new Error("PasswordRequiresUpper", "Passwords must have at least one uppercase ('A'-'Z')."));
      if (settings.RequireDigit && !chars.Keys.Any(char.IsDigit))
        errors.Add(new Error("PasswordRequiresDigit", "Passwords must have at least one digit ('0'-'9')."));

      if (errors.Any())
      {
        throw new InvalidPasswordException(passwordString, errors);
      }
    }
  }
}
