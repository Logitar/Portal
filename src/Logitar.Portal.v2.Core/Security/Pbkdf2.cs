using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;

namespace Logitar.Portal.v2.Core.Security;

public class Pbkdf2
{
  private const char Separator = ':';

  private readonly KeyDerivationPrf _algorithm = KeyDerivationPrf.HMACSHA256;
  private readonly int _iterationCount = 100000;
  private readonly byte[] _salt = RandomNumberGenerator.GetBytes(32);
  private readonly byte[] _hash;

  public Pbkdf2(string password)
  {
    _hash = ComputeHash(password);
  }

  private Pbkdf2(KeyDerivationPrf algorithm, int iterationCount, byte[] salt, byte[] hash)
  {
    _algorithm = algorithm;
    _iterationCount = iterationCount;
    _salt = salt;
    _hash = hash;
  }

  public static Pbkdf2 Parse(string s)
  {
    string[] values = s.Split(Separator);
    if (values.Length != 4)
    {
      throw new ArgumentException($"The value '{s}' is not a valid PBKDF2 string representation.", nameof(s));
    }

    return new Pbkdf2(Enum.Parse<KeyDerivationPrf>(values[0]), int.Parse(values[1]),
      Convert.FromBase64String(values[2]), Convert.FromBase64String(values[3]));
  }
  public static bool TryParse(string s, out Pbkdf2? pbkdf2)
  {
    try
    {
      pbkdf2 = Parse(s);
      return true;
    }
    catch (Exception)
    {
      pbkdf2 = null;
      return false;
    }
  }

  public bool IsMatch(string password) => _hash.SequenceEqual(ComputeHash(password, _hash.Length));

  private byte[] ComputeHash(string password, int? length = null)
  {
    return KeyDerivation.Pbkdf2(password, _salt, _algorithm, _iterationCount, length ?? _salt.Length);
  }

  public override bool Equals(object? obj) => obj is Pbkdf2 pbkdf2 && pbkdf2._algorithm == _algorithm
    && pbkdf2._iterationCount == _iterationCount
    && pbkdf2._salt.SequenceEqual(_salt)
    && pbkdf2._hash.SequenceEqual(_hash);
  public override int GetHashCode() => HashCode.Combine(_algorithm, _iterationCount,
    Convert.ToBase64String(_salt), Convert.ToBase64String(_hash));
  public override string ToString() => string.Join(Separator, _algorithm, _iterationCount,
    Convert.ToBase64String(_salt), Convert.ToBase64String(_hash));
}
